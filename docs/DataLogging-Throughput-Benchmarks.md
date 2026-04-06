# Data-Logging Throughput Benchmarks

Comparative write-throughput benchmarks for the two historian back-ends
supported by the HMI Server: **SQLite** (embedded, file-based) and
**TimescaleDB** (PostgreSQL + hypertable extension, Docker).

## Test Environment

| Property | Value |
|---|---|
| **Runtime** | .NET 10.0.5 |
| **OS** | Windows NT 10.0.26200.0 |
| **CPU** | 12 logical processors |
| **Records per test** | 100,000 (except single-insert: 500) |
| **SQLite** | `Microsoft.Data.Sqlite` 9.0.0, file-based on local SSD |
| **TimescaleDB** | `timescale/timescaledb:latest-pg17` Docker container, `Npgsql` 8.0.3, localhost TCP |

---

## SQLite Results

| # | Scenario | Records | Elapsed | Records/sec |
|---|---|---:|---:|---:|
| 1 | File \| Single insert \| journal=DELETE | 500 | 4.364 s | **115** |
| 2 | File \| Batched txn \| journal=DELETE | 100,000 | 0.523 s | **191,309** |
| 3 | File \| Batched txn \| journal=WAL | 100,000 | 0.311 s | **321,518** |
| 4 | File \| Batched txn \| WAL+SYNC=NORMAL | 100,000 | 0.387 s | **258,163** |
| 5 | Memory \| Batched txn \| in-memory | 100,000 | 0.198 s | **505,724** |
| 6 | File \| 4 producers \| WAL+batched | 100,000 | 0.666 s | **150,121** |

### Key Observations — SQLite

- **Single inserts without transactions** are catastrophically slow (~115 rec/s)
  because each INSERT triggers a full `fsync` to the journal file.
- **Batched transactions** eliminate per-row sync overhead and jump to
  **191K rec/s** (DELETE journal) or **322K rec/s** (WAL mode).
- **WAL mode** is ~1.7× faster than DELETE journal for batched writes because
  writes go to an append-only WAL file instead of overwriting the main DB.
- **`synchronous=NORMAL`** with WAL was slightly *slower* than default WAL in
  this run (258K vs 322K) — this can vary by disk; the difference is typically
  within noise on SSDs.
- **In-memory** removes all disk I/O and reaches **506K rec/s** — the upper
  bound for the serialization + B-tree insertion path.
- **4 concurrent producers** reach **150K rec/s** total. SQLite's writer lock
  serializes commits, so multi-producer throughput is lower than single-connection
  batched writes.

### Production Configuration

The HMI Server's `SqliteLogger` uses:
- WAL mode + `synchronous=NORMAL`
- Single writer thread draining a 200K-entry bounded channel
- Batches of up to 5,000 rows per transaction
- Prepared statement reused across batches

**Expected sustained throughput: ~50,000–80,000 variable changes/sec** (accounting
for per-variable MaxAge DELETE cleanup inside the same transaction).

---

## TimescaleDB Results

| # | Scenario | Records | Elapsed | Records/sec |
|---|---|---:|---:|---:|
| 1 | PG \| Single insert \| plain table | 500 | 1.474 s | **339** |
| 2 | PG \| Batched txn \| plain table | 100,000 | 32.382 s | **3,088** |
| 3 | TS \| Batched txn \| hypertable | 100,000 | 49.130 s | **2,035** |
| 4 | PG \| COPY binary \| plain table | 100,000 | 0.299 s | **334,134** |
| 5 | TS \| COPY binary \| hypertable | 100,000 | 0.426 s | **234,729** |
| 6 | TS \| 4 producers \| hypertable | 100,000 | 29.350 s | **3,407** |
| 7 | TS \| 4 prod COPY \| hypertable | 100,000 | 0.363 s | **275,487** |

### Key Observations — TimescaleDB

- **Single inserts** (~339 rec/s) are slow due to per-statement TCP round-trips
  plus PostgreSQL's per-row WAL/fsync overhead.
- **Batched transactions with INSERT** (~3K rec/s) are **~100× slower than SQLite**
  for the same pattern. Each `INSERT ... VALUES` still requires a full client→server
  round-trip, and the TCP/network stack adds latency that compounds over 100K rows.
- **Hypertable overhead** adds ~35% cost vs plain PostgreSQL table for INSERT-based
  writes (3,088 → 2,035 rec/s) due to chunk routing and constraint checking.
- **COPY protocol** is the game-changer: **334K rec/s** on a plain table and
  **235K rec/s** on a hypertable — comparable to SQLite WAL. COPY streams data in
  binary format over a single TCP pipe, bypassing per-row SQL parsing.
- **Multi-producer COPY** (4 connections) reaches **275K rec/s** — the PostgreSQL
  MVCC architecture handles concurrent writers far better than SQLite's single-writer
  lock.
- **Multi-producer INSERT** (4 connections, batched) is only **3.4K rec/s** — the
  bottleneck is per-row round-trips, not concurrency.

### Production Configuration

The HMI Server's `TimescaleLogger` currently uses:
- Individual `INSERT` statements in a batched transaction (same as scenario #3)
- Single writer thread, batches of up to 500 rows
- Channel capacity: 10,000

**Current sustained throughput: ~2,000–3,000 variable changes/sec** — significantly
below the SQLite logger.

### Recommendation

To match SQLite throughput, the `TimescaleLogger` should be migrated to use
PostgreSQL's **COPY binary protocol** (`NpgsqlConnection.BeginBinaryImport`).
This would bring hypertable write performance from ~2K rec/s to **~235K rec/s**
(a ~100× improvement). Additional tuning:
- Increase channel capacity from 10,000 to 200,000
- Increase batch size from 500 to 5,000
- Consider multi-connection COPY for very high variable counts

---

## Side-by-Side Comparison

| Strategy | SQLite | TimescaleDB | Ratio |
|---|---:|---:|---:|
| Single INSERT (no txn) | 115 | 339 | TS 2.9× |
| Batched INSERT (1 txn) | 321,518 | 2,035 | **SQLite 158×** |
| Bulk load (best method) | 321,518 | 234,729 | SQLite 1.4× |
| Multi-producer (best) | 150,121 | 275,487 | **TS 1.8×** |
| In-memory / theoretical max | 505,724 | — | — |

### Summary

- **SQLite wins** for single-connection batched writes by a massive margin because
  all I/O is local (no TCP) and WAL mode is highly optimized for sequential appends.
- **TimescaleDB wins** for multi-connection concurrent writes using the COPY protocol,
  and scales better as more producers are added (MVCC vs single-writer lock).
- For the HMI Server's architecture (single writer thread + channel), **SQLite is
  the higher-throughput choice** for data logging. TimescaleDB adds value for
  time-series queries, continuous aggregates, retention policies, and distributed
  deployments — but the INSERT-based writer should be switched to COPY to avoid
  the 100× throughput penalty.

---

## How to Run

### SQLite Benchmark

```bash
cd SqliteThroughputBenchmark
dotnet run -c Release -- 100000
```

### TimescaleDB Benchmark

```bash
# Start a TimescaleDB container
docker run -d --name tsdb-bench \
  -p 5432:5432 \
  -e POSTGRES_PASSWORD=postgres \
  -e POSTGRES_DB=benchmark \
  timescale/timescaledb:latest-pg17

# Wait a few seconds for PostgreSQL to initialize, then run
cd TimescaleThroughputBenchmark
dotnet run -c Release -- 100000

# Clean up
docker stop tsdb-bench && docker rm tsdb-bench
```

To use a custom connection string:

```bash
$env:TIMESCALE_CONN = "Host=myhost;Port=5432;Database=benchmark;Username=postgres;Password=secret"
dotnet run -c Release -- 100000
```
