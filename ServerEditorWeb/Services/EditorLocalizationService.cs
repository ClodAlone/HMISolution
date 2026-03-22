using System.Text.Json;

namespace ServerEditorWeb.Services;

/// <summary>
/// Provides editor UI localization. Persists the selected locale to disk.
/// All translations are embedded — no resource files needed.
/// </summary>
public class EditorLocalizationService
{
    private static readonly string SettingsPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "SimpleOpcFileServer", "editor_locale.json");

    public string Locale { get; private set; } = "en";

    public event Action? LocaleChanged;

    /// <summary>Supported locales: code → (display name, flag).</summary>
    public static readonly (string Code, string Name, string Flag)[] SupportedLocales =
    [
        ("en", "English", "🇬🇧"),
        ("de", "Deutsch", "🇩🇪"),
        ("it", "Italiano", "🇮🇹"),
        ("fr", "Français", "🇫🇷"),
        ("ja", "日本語", "🇯🇵"),
        ("zh", "中文", "🇨🇳"),
    ];

    public EditorLocalizationService()
    {
        Load();
    }

    public void SetLocale(string locale)
    {
        if (Locale == locale) return;
        Locale = locale;
        Save();
        LocaleChanged?.Invoke();
    }

    /// <summary>
    /// Resolve a localization key. Returns the translated string for the current locale,
    /// falling back to English if the key is missing.
    /// </summary>
    public string this[string key]
    {
        get
        {
            if (_translations.TryGetValue(Locale, out var dict) && dict.TryGetValue(key, out var val))
                return val;
            if (_translations.TryGetValue("en", out var enDict) && enDict.TryGetValue(key, out var enVal))
                return enVal;
            return key; // fallback: return the key itself
        }
    }

    private void Load()
    {
        try
        {
            if (File.Exists(SettingsPath))
            {
                var json = File.ReadAllText(SettingsPath);
                var obj = JsonSerializer.Deserialize<LocaleSettings>(json);
                if (obj != null && SupportedLocales.Any(l => l.Code == obj.Locale))
                    Locale = obj.Locale;
            }
        }
        catch { /* use default */ }
    }

    private void Save()
    {
        try
        {
            var dir = Path.GetDirectoryName(SettingsPath)!;
            Directory.CreateDirectory(dir);
            var json = JsonSerializer.Serialize(new LocaleSettings { Locale = Locale });
            File.WriteAllText(SettingsPath, json);
        }
        catch { /* ignore */ }
    }

    private record LocaleSettings
    {
        public string Locale { get; init; } = "en";
    }

    // ═══════════════════════════════════════════════════════════
    //  Translation dictionaries
    // ═══════════════════════════════════════════════════════════

    private static readonly Dictionary<string, Dictionary<string, string>> _translations = new()
    {
        ["en"] = new()
        {
            // ── Menus ──
            ["menu.file"] = "File",
            ["menu.edit"] = "Edit",
            ["menu.view"] = "View",
            ["menu.settings"] = "Settings",
            ["menu.help"] = "Help",

            // File menu
            ["menu.file.new"] = "New",
            ["menu.file.open"] = "Open",
            ["menu.file.browseServer"] = "Browse Server…",
            ["menu.file.recent"] = "Recent",
            ["menu.file.save"] = "Save",
            ["menu.file.saveAs"] = "Save As…",

            // Edit menu
            ["menu.edit.addFolder"] = "Add Folder",
            ["menu.edit.addVariable"] = "Add Variable",
            ["menu.edit.addScript"] = "Add Script",
            ["menu.edit.addScreen"] = "Add Screen",
            ["menu.edit.copy"] = "Copy",
            ["menu.edit.paste"] = "Paste",
            ["menu.edit.delete"] = "Delete",

            // View menu
            ["menu.view.panels"] = "Panels",
            ["menu.view.maximize"] = "Maximize Active Panel",

            // Settings menu
            ["menu.settings.theme"] = "Theme",
            ["menu.settings.light"] = "Light",
            ["menu.settings.dark"] = "Dark",
            ["menu.settings.language"] = "Language",
            ["menu.settings.protection"] = "Project Protection",
            ["menu.settings.unlock"] = "Unlock Project…",
            ["menu.settings.lock"] = "Lock Project",
            ["menu.settings.changePw"] = "Change Password…",
            ["menu.settings.removePw"] = "Remove Password…",
            ["menu.settings.setPw"] = "Set Password…",

            // Help menu
            ["menu.help.help"] = "Help",
            ["menu.help.validate"] = "Validate Project",
            ["menu.help.about"] = "About",

            // Toolbar tooltips
            ["toolbar.new"] = "New",
            ["toolbar.save"] = "Save",
            ["toolbar.saveAs"] = "Save As…",
            ["toolbar.undo"] = "Undo",
            ["toolbar.redo"] = "Redo",
            ["toolbar.copy"] = "Copy",
            ["toolbar.paste"] = "Paste",
            ["toolbar.delete"] = "Delete",
            ["toolbar.addFolder"] = "Add Folder",
            ["toolbar.addVariable"] = "Add Variable",
            ["toolbar.addScript"] = "Add Script",
            ["toolbar.addPlc"] = "Add PLC Program",
            ["toolbar.addRecipe"] = "Add Recipe",
            ["toolbar.addScheduler"] = "Add Scheduler",
            ["toolbar.addReport"] = "Add Report",
            ["toolbar.addScreen"] = "Add Screen",
            ["toolbar.restore"] = "Restore layout",
            ["toolbar.maximize"] = "Maximize active panel",
            ["toolbar.toggleTheme"] = "Toggle theme",
            ["toolbar.logout"] = "Logout",

            // Status bar
            ["status.unsaved"] = "Unsaved",
            ["status.locked"] = "Locked",
            ["status.unlocked"] = "Unlocked",

            // Dialogs
            ["dialog.saveAs.title"] = "Save As",
            ["dialog.saveAs.filePath"] = "File path:",
            ["dialog.saveAs.current"] = "Current:",
            ["dialog.saveAs.cancel"] = "Cancel",
            ["dialog.saveAs.save"] = "Save",

            ["dialog.unsaved.title"] = "Unsaved Changes",
            ["dialog.unsaved.message"] = "The project {0} has unsaved changes. Do you want to save before closing?",
            ["dialog.unsaved.cancel"] = "Cancel",
            ["dialog.unsaved.discard"] = "Discard",
            ["dialog.unsaved.save"] = "Save",

            ["dialog.open.title"] = "Open Project File",
            ["dialog.open.empty"] = "Select a drive or enter a path",
            ["dialog.open.emptyFolder"] = "Empty folder or access denied",
            ["dialog.open.cancel"] = "Cancel",
            ["dialog.open.open"] = "Open",

            ["dialog.unlock.title"] = "Unlock Project",
            ["dialog.unlock.message"] = "This project is password-protected. Enter the password to unlock it for editing.",
            ["dialog.unlock.password"] = "Password",
            ["dialog.unlock.cancel"] = "Cancel",
            ["dialog.unlock.unlock"] = "Unlock",

            ["dialog.setPw.title.change"] = "Change Project Password",
            ["dialog.setPw.title.set"] = "Set Project Password",
            ["dialog.setPw.msgChange"] = "Change or remove the project protection password.",
            ["dialog.setPw.msgSet"] = "Set a password to protect this project. When locked, the project cannot be edited without the correct password.",
            ["dialog.setPw.currentPw"] = "Current Password",
            ["dialog.setPw.newPw"] = "New Password",
            ["dialog.setPw.confirmPw"] = "Confirm Password",
            ["dialog.setPw.cancel"] = "Cancel",
            ["dialog.setPw.change"] = "Change Password",
            ["dialog.setPw.set"] = "Set Password",

            ["dialog.changePw.title"] = "Change Password",
            ["dialog.changePw.message"] = "You must change your password before continuing.",
            ["dialog.changePw.currentPw"] = "Current Password",
            ["dialog.changePw.newPw"] = "New Password",
            ["dialog.changePw.confirmPw"] = "Confirm New Password",
            ["dialog.changePw.submit"] = "Change Password",

            // General
            ["general.serverEditor"] = "Server Editor",
            ["general.projectLocked"] = "Project is locked. Unlock it first to make changes.",
            ["general.projectUnlocked"] = "Project unlocked.",
            ["general.projectLocked2"] = "Project locked.",
        },

        ["de"] = new()
        {
            ["menu.file"] = "Datei",
            ["menu.edit"] = "Bearbeiten",
            ["menu.view"] = "Ansicht",
            ["menu.settings"] = "Einstellungen",
            ["menu.help"] = "Hilfe",

            ["menu.file.new"] = "Neu",
            ["menu.file.open"] = "Öffnen",
            ["menu.file.browseServer"] = "Server durchsuchen…",
            ["menu.file.recent"] = "Zuletzt verwendet",
            ["menu.file.save"] = "Speichern",
            ["menu.file.saveAs"] = "Speichern unter…",

            ["menu.edit.addFolder"] = "Ordner hinzufügen",
            ["menu.edit.addVariable"] = "Variable hinzufügen",
            ["menu.edit.addScript"] = "Skript hinzufügen",
            ["menu.edit.addScreen"] = "Bildschirm hinzufügen",
            ["menu.edit.copy"] = "Kopieren",
            ["menu.edit.paste"] = "Einfügen",
            ["menu.edit.delete"] = "Löschen",

            ["menu.view.panels"] = "Bereiche",
            ["menu.view.maximize"] = "Aktiven Bereich maximieren",

            ["menu.settings.theme"] = "Design",
            ["menu.settings.light"] = "Hell",
            ["menu.settings.dark"] = "Dunkel",
            ["menu.settings.language"] = "Sprache",
            ["menu.settings.protection"] = "Projektschutz",
            ["menu.settings.unlock"] = "Projekt entsperren…",
            ["menu.settings.lock"] = "Projekt sperren",
            ["menu.settings.changePw"] = "Passwort ändern…",
            ["menu.settings.removePw"] = "Passwort entfernen…",
            ["menu.settings.setPw"] = "Passwort festlegen…",

            ["menu.help.help"] = "Hilfe",
            ["menu.help.validate"] = "Projekt validieren",
            ["menu.help.about"] = "Über",

            ["toolbar.new"] = "Neu",
            ["toolbar.save"] = "Speichern",
            ["toolbar.saveAs"] = "Speichern unter…",
            ["toolbar.undo"] = "Rückgängig",
            ["toolbar.redo"] = "Wiederholen",
            ["toolbar.copy"] = "Kopieren",
            ["toolbar.paste"] = "Einfügen",
            ["toolbar.delete"] = "Löschen",
            ["toolbar.addFolder"] = "Ordner hinzufügen",
            ["toolbar.addVariable"] = "Variable hinzufügen",
            ["toolbar.addScript"] = "Skript hinzufügen",
            ["toolbar.addPlc"] = "SPS-Programm hinzufügen",
            ["toolbar.addRecipe"] = "Rezept hinzufügen",
            ["toolbar.addScheduler"] = "Zeitplan hinzufügen",
            ["toolbar.addReport"] = "Bericht hinzufügen",
            ["toolbar.addScreen"] = "Bildschirm hinzufügen",
            ["toolbar.restore"] = "Layout wiederherstellen",
            ["toolbar.maximize"] = "Aktiven Bereich maximieren",
            ["toolbar.toggleTheme"] = "Design wechseln",
            ["toolbar.logout"] = "Abmelden",

            ["status.unsaved"] = "Nicht gespeichert",
            ["status.locked"] = "Gesperrt",
            ["status.unlocked"] = "Entsperrt",

            ["dialog.saveAs.title"] = "Speichern unter",
            ["dialog.saveAs.filePath"] = "Dateipfad:",
            ["dialog.saveAs.current"] = "Aktuell:",
            ["dialog.saveAs.cancel"] = "Abbrechen",
            ["dialog.saveAs.save"] = "Speichern",

            ["dialog.unsaved.title"] = "Nicht gespeicherte Änderungen",
            ["dialog.unsaved.message"] = "Das Projekt {0} hat nicht gespeicherte Änderungen. Möchten Sie vor dem Schließen speichern?",
            ["dialog.unsaved.cancel"] = "Abbrechen",
            ["dialog.unsaved.discard"] = "Verwerfen",
            ["dialog.unsaved.save"] = "Speichern",

            ["dialog.open.title"] = "Projektdatei öffnen",
            ["dialog.open.empty"] = "Wählen Sie ein Laufwerk oder geben Sie einen Pfad ein",
            ["dialog.open.emptyFolder"] = "Leerer Ordner oder Zugriff verweigert",
            ["dialog.open.cancel"] = "Abbrechen",
            ["dialog.open.open"] = "Öffnen",

            ["dialog.unlock.title"] = "Projekt entsperren",
            ["dialog.unlock.message"] = "Dieses Projekt ist passwortgeschützt. Geben Sie das Passwort ein, um es zu bearbeiten.",
            ["dialog.unlock.password"] = "Passwort",
            ["dialog.unlock.cancel"] = "Abbrechen",
            ["dialog.unlock.unlock"] = "Entsperren",

            ["dialog.setPw.title.change"] = "Projektpasswort ändern",
            ["dialog.setPw.title.set"] = "Projektpasswort festlegen",
            ["dialog.setPw.msgChange"] = "Projektschutzpasswort ändern oder entfernen.",
            ["dialog.setPw.msgSet"] = "Legen Sie ein Passwort fest, um dieses Projekt zu schützen.",
            ["dialog.setPw.currentPw"] = "Aktuelles Passwort",
            ["dialog.setPw.newPw"] = "Neues Passwort",
            ["dialog.setPw.confirmPw"] = "Passwort bestätigen",
            ["dialog.setPw.cancel"] = "Abbrechen",
            ["dialog.setPw.change"] = "Passwort ändern",
            ["dialog.setPw.set"] = "Passwort festlegen",

            ["dialog.changePw.title"] = "Passwort ändern",
            ["dialog.changePw.message"] = "Sie müssen Ihr Passwort ändern, bevor Sie fortfahren können.",
            ["dialog.changePw.currentPw"] = "Aktuelles Passwort",
            ["dialog.changePw.newPw"] = "Neues Passwort",
            ["dialog.changePw.confirmPw"] = "Neues Passwort bestätigen",
            ["dialog.changePw.submit"] = "Passwort ändern",

            ["general.serverEditor"] = "Server-Editor",
            ["general.projectLocked"] = "Projekt ist gesperrt. Entsperren Sie es zuerst.",
            ["general.projectUnlocked"] = "Projekt entsperrt.",
            ["general.projectLocked2"] = "Projekt gesperrt.",
        },

        ["it"] = new()
        {
            ["menu.file"] = "File",
            ["menu.edit"] = "Modifica",
            ["menu.view"] = "Visualizza",
            ["menu.settings"] = "Impostazioni",
            ["menu.help"] = "Aiuto",

            ["menu.file.new"] = "Nuovo",
            ["menu.file.open"] = "Apri",
            ["menu.file.browseServer"] = "Sfoglia server…",
            ["menu.file.recent"] = "Recenti",
            ["menu.file.save"] = "Salva",
            ["menu.file.saveAs"] = "Salva con nome…",

            ["menu.edit.addFolder"] = "Aggiungi cartella",
            ["menu.edit.addVariable"] = "Aggiungi variabile",
            ["menu.edit.addScript"] = "Aggiungi script",
            ["menu.edit.addScreen"] = "Aggiungi schermata",
            ["menu.edit.copy"] = "Copia",
            ["menu.edit.paste"] = "Incolla",
            ["menu.edit.delete"] = "Elimina",

            ["menu.view.panels"] = "Pannelli",
            ["menu.view.maximize"] = "Massimizza pannello attivo",

            ["menu.settings.theme"] = "Tema",
            ["menu.settings.light"] = "Chiaro",
            ["menu.settings.dark"] = "Scuro",
            ["menu.settings.language"] = "Lingua",
            ["menu.settings.protection"] = "Protezione progetto",
            ["menu.settings.unlock"] = "Sblocca progetto…",
            ["menu.settings.lock"] = "Blocca progetto",
            ["menu.settings.changePw"] = "Cambia password…",
            ["menu.settings.removePw"] = "Rimuovi password…",
            ["menu.settings.setPw"] = "Imposta password…",

            ["menu.help.help"] = "Aiuto",
            ["menu.help.validate"] = "Valida progetto",
            ["menu.help.about"] = "Informazioni",

            ["toolbar.new"] = "Nuovo",
            ["toolbar.save"] = "Salva",
            ["toolbar.saveAs"] = "Salva con nome…",
            ["toolbar.undo"] = "Annulla",
            ["toolbar.redo"] = "Ripeti",
            ["toolbar.copy"] = "Copia",
            ["toolbar.paste"] = "Incolla",
            ["toolbar.delete"] = "Elimina",
            ["toolbar.addFolder"] = "Aggiungi cartella",
            ["toolbar.addVariable"] = "Aggiungi variabile",
            ["toolbar.addScript"] = "Aggiungi script",
            ["toolbar.addPlc"] = "Aggiungi programma PLC",
            ["toolbar.addRecipe"] = "Aggiungi ricetta",
            ["toolbar.addScheduler"] = "Aggiungi pianificazione",
            ["toolbar.addReport"] = "Aggiungi report",
            ["toolbar.addScreen"] = "Aggiungi schermata",
            ["toolbar.restore"] = "Ripristina layout",
            ["toolbar.maximize"] = "Massimizza pannello attivo",
            ["toolbar.toggleTheme"] = "Cambia tema",
            ["toolbar.logout"] = "Esci",

            ["status.unsaved"] = "Non salvato",
            ["status.locked"] = "Bloccato",
            ["status.unlocked"] = "Sbloccato",

            ["dialog.saveAs.title"] = "Salva con nome",
            ["dialog.saveAs.filePath"] = "Percorso file:",
            ["dialog.saveAs.current"] = "Corrente:",
            ["dialog.saveAs.cancel"] = "Annulla",
            ["dialog.saveAs.save"] = "Salva",

            ["dialog.unsaved.title"] = "Modifiche non salvate",
            ["dialog.unsaved.message"] = "Il progetto {0} ha modifiche non salvate. Vuoi salvare prima di chiudere?",
            ["dialog.unsaved.cancel"] = "Annulla",
            ["dialog.unsaved.discard"] = "Scarta",
            ["dialog.unsaved.save"] = "Salva",

            ["dialog.open.title"] = "Apri file progetto",
            ["dialog.open.empty"] = "Seleziona un'unità o inserisci un percorso",
            ["dialog.open.emptyFolder"] = "Cartella vuota o accesso negato",
            ["dialog.open.cancel"] = "Annulla",
            ["dialog.open.open"] = "Apri",

            ["dialog.unlock.title"] = "Sblocca progetto",
            ["dialog.unlock.message"] = "Questo progetto è protetto da password. Inserisci la password per modificarlo.",
            ["dialog.unlock.password"] = "Password",
            ["dialog.unlock.cancel"] = "Annulla",
            ["dialog.unlock.unlock"] = "Sblocca",

            ["dialog.setPw.title.change"] = "Cambia password progetto",
            ["dialog.setPw.title.set"] = "Imposta password progetto",
            ["dialog.setPw.msgChange"] = "Cambia o rimuovi la password di protezione del progetto.",
            ["dialog.setPw.msgSet"] = "Imposta una password per proteggere questo progetto.",
            ["dialog.setPw.currentPw"] = "Password corrente",
            ["dialog.setPw.newPw"] = "Nuova password",
            ["dialog.setPw.confirmPw"] = "Conferma password",
            ["dialog.setPw.cancel"] = "Annulla",
            ["dialog.setPw.change"] = "Cambia password",
            ["dialog.setPw.set"] = "Imposta password",

            ["dialog.changePw.title"] = "Cambia password",
            ["dialog.changePw.message"] = "Devi cambiare la password prima di continuare.",
            ["dialog.changePw.currentPw"] = "Password corrente",
            ["dialog.changePw.newPw"] = "Nuova password",
            ["dialog.changePw.confirmPw"] = "Conferma nuova password",
            ["dialog.changePw.submit"] = "Cambia password",

            ["general.serverEditor"] = "Editor Server",
            ["general.projectLocked"] = "Il progetto è bloccato. Sbloccalo prima di apportare modifiche.",
            ["general.projectUnlocked"] = "Progetto sbloccato.",
            ["general.projectLocked2"] = "Progetto bloccato.",
        },

        ["fr"] = new()
        {
            ["menu.file"] = "Fichier",
            ["menu.edit"] = "Édition",
            ["menu.view"] = "Affichage",
            ["menu.settings"] = "Paramètres",
            ["menu.help"] = "Aide",

            ["menu.file.new"] = "Nouveau",
            ["menu.file.open"] = "Ouvrir",
            ["menu.file.browseServer"] = "Parcourir le serveur…",
            ["menu.file.recent"] = "Récents",
            ["menu.file.save"] = "Enregistrer",
            ["menu.file.saveAs"] = "Enregistrer sous…",

            ["menu.edit.addFolder"] = "Ajouter un dossier",
            ["menu.edit.addVariable"] = "Ajouter une variable",
            ["menu.edit.addScript"] = "Ajouter un script",
            ["menu.edit.addScreen"] = "Ajouter un écran",
            ["menu.edit.copy"] = "Copier",
            ["menu.edit.paste"] = "Coller",
            ["menu.edit.delete"] = "Supprimer",

            ["menu.view.panels"] = "Panneaux",
            ["menu.view.maximize"] = "Maximiser le panneau actif",

            ["menu.settings.theme"] = "Thème",
            ["menu.settings.light"] = "Clair",
            ["menu.settings.dark"] = "Sombre",
            ["menu.settings.language"] = "Langue",
            ["menu.settings.protection"] = "Protection du projet",
            ["menu.settings.unlock"] = "Déverrouiller le projet…",
            ["menu.settings.lock"] = "Verrouiller le projet",
            ["menu.settings.changePw"] = "Changer le mot de passe…",
            ["menu.settings.removePw"] = "Supprimer le mot de passe…",
            ["menu.settings.setPw"] = "Définir un mot de passe…",

            ["menu.help.help"] = "Aide",
            ["menu.help.validate"] = "Valider le projet",
            ["menu.help.about"] = "À propos",

            ["toolbar.new"] = "Nouveau",
            ["toolbar.save"] = "Enregistrer",
            ["toolbar.saveAs"] = "Enregistrer sous…",
            ["toolbar.undo"] = "Annuler",
            ["toolbar.redo"] = "Rétablir",
            ["toolbar.copy"] = "Copier",
            ["toolbar.paste"] = "Coller",
            ["toolbar.delete"] = "Supprimer",
            ["toolbar.addFolder"] = "Ajouter un dossier",
            ["toolbar.addVariable"] = "Ajouter une variable",
            ["toolbar.addScript"] = "Ajouter un script",
            ["toolbar.addPlc"] = "Ajouter un programme API",
            ["toolbar.addRecipe"] = "Ajouter une recette",
            ["toolbar.addScheduler"] = "Ajouter un planificateur",
            ["toolbar.addReport"] = "Ajouter un rapport",
            ["toolbar.addScreen"] = "Ajouter un écran",
            ["toolbar.restore"] = "Restaurer la disposition",
            ["toolbar.maximize"] = "Maximiser le panneau actif",
            ["toolbar.toggleTheme"] = "Changer le thème",
            ["toolbar.logout"] = "Déconnexion",

            ["status.unsaved"] = "Non enregistré",
            ["status.locked"] = "Verrouillé",
            ["status.unlocked"] = "Déverrouillé",

            ["dialog.saveAs.title"] = "Enregistrer sous",
            ["dialog.saveAs.filePath"] = "Chemin du fichier :",
            ["dialog.saveAs.current"] = "Actuel :",
            ["dialog.saveAs.cancel"] = "Annuler",
            ["dialog.saveAs.save"] = "Enregistrer",

            ["dialog.unsaved.title"] = "Modifications non enregistrées",
            ["dialog.unsaved.message"] = "Le projet {0} a des modifications non enregistrées. Voulez-vous enregistrer avant de fermer ?",
            ["dialog.unsaved.cancel"] = "Annuler",
            ["dialog.unsaved.discard"] = "Abandonner",
            ["dialog.unsaved.save"] = "Enregistrer",

            ["dialog.open.title"] = "Ouvrir un fichier projet",
            ["dialog.open.empty"] = "Sélectionnez un lecteur ou entrez un chemin",
            ["dialog.open.emptyFolder"] = "Dossier vide ou accès refusé",
            ["dialog.open.cancel"] = "Annuler",
            ["dialog.open.open"] = "Ouvrir",

            ["dialog.unlock.title"] = "Déverrouiller le projet",
            ["dialog.unlock.message"] = "Ce projet est protégé par mot de passe. Entrez le mot de passe pour le modifier.",
            ["dialog.unlock.password"] = "Mot de passe",
            ["dialog.unlock.cancel"] = "Annuler",
            ["dialog.unlock.unlock"] = "Déverrouiller",

            ["dialog.setPw.title.change"] = "Changer le mot de passe du projet",
            ["dialog.setPw.title.set"] = "Définir le mot de passe du projet",
            ["dialog.setPw.msgChange"] = "Changer ou supprimer le mot de passe de protection du projet.",
            ["dialog.setPw.msgSet"] = "Définissez un mot de passe pour protéger ce projet.",
            ["dialog.setPw.currentPw"] = "Mot de passe actuel",
            ["dialog.setPw.newPw"] = "Nouveau mot de passe",
            ["dialog.setPw.confirmPw"] = "Confirmer le mot de passe",
            ["dialog.setPw.cancel"] = "Annuler",
            ["dialog.setPw.change"] = "Changer le mot de passe",
            ["dialog.setPw.set"] = "Définir le mot de passe",

            ["dialog.changePw.title"] = "Changer le mot de passe",
            ["dialog.changePw.message"] = "Vous devez changer votre mot de passe avant de continuer.",
            ["dialog.changePw.currentPw"] = "Mot de passe actuel",
            ["dialog.changePw.newPw"] = "Nouveau mot de passe",
            ["dialog.changePw.confirmPw"] = "Confirmer le nouveau mot de passe",
            ["dialog.changePw.submit"] = "Changer le mot de passe",

            ["general.serverEditor"] = "Éditeur serveur",
            ["general.projectLocked"] = "Le projet est verrouillé. Déverrouillez-le d'abord.",
            ["general.projectUnlocked"] = "Projet déverrouillé.",
            ["general.projectLocked2"] = "Projet verrouillé.",
        },

        ["ja"] = new()
        {
            ["menu.file"] = "ファイル",
            ["menu.edit"] = "編集",
            ["menu.view"] = "表示",
            ["menu.settings"] = "設定",
            ["menu.help"] = "ヘルプ",

            ["menu.file.new"] = "新規作成",
            ["menu.file.open"] = "開く",
            ["menu.file.browseServer"] = "サーバーを参照…",
            ["menu.file.recent"] = "最近使用",
            ["menu.file.save"] = "保存",
            ["menu.file.saveAs"] = "名前を付けて保存…",

            ["menu.edit.addFolder"] = "フォルダーを追加",
            ["menu.edit.addVariable"] = "変数を追加",
            ["menu.edit.addScript"] = "スクリプトを追加",
            ["menu.edit.addScreen"] = "画面を追加",
            ["menu.edit.copy"] = "コピー",
            ["menu.edit.paste"] = "貼り付け",
            ["menu.edit.delete"] = "削除",

            ["menu.view.panels"] = "パネル",
            ["menu.view.maximize"] = "アクティブパネルを最大化",

            ["menu.settings.theme"] = "テーマ",
            ["menu.settings.light"] = "ライト",
            ["menu.settings.dark"] = "ダーク",
            ["menu.settings.language"] = "言語",
            ["menu.settings.protection"] = "プロジェクト保護",
            ["menu.settings.unlock"] = "プロジェクトのロック解除…",
            ["menu.settings.lock"] = "プロジェクトをロック",
            ["menu.settings.changePw"] = "パスワードを変更…",
            ["menu.settings.removePw"] = "パスワードを削除…",
            ["menu.settings.setPw"] = "パスワードを設定…",

            ["menu.help.help"] = "ヘルプ",
            ["menu.help.validate"] = "プロジェクトを検証",
            ["menu.help.about"] = "バージョン情報",

            ["toolbar.new"] = "新規",
            ["toolbar.save"] = "保存",
            ["toolbar.saveAs"] = "名前を付けて保存…",
            ["toolbar.undo"] = "元に戻す",
            ["toolbar.redo"] = "やり直し",
            ["toolbar.copy"] = "コピー",
            ["toolbar.paste"] = "貼り付け",
            ["toolbar.delete"] = "削除",
            ["toolbar.addFolder"] = "フォルダーを追加",
            ["toolbar.addVariable"] = "変数を追加",
            ["toolbar.addScript"] = "スクリプトを追加",
            ["toolbar.addPlc"] = "PLCプログラムを追加",
            ["toolbar.addRecipe"] = "レシピを追加",
            ["toolbar.addScheduler"] = "スケジューラーを追加",
            ["toolbar.addReport"] = "レポートを追加",
            ["toolbar.addScreen"] = "画面を追加",
            ["toolbar.restore"] = "レイアウトを復元",
            ["toolbar.maximize"] = "アクティブパネルを最大化",
            ["toolbar.toggleTheme"] = "テーマ切替",
            ["toolbar.logout"] = "ログアウト",

            ["status.unsaved"] = "未保存",
            ["status.locked"] = "ロック中",
            ["status.unlocked"] = "ロック解除",

            ["dialog.saveAs.title"] = "名前を付けて保存",
            ["dialog.saveAs.filePath"] = "ファイルパス：",
            ["dialog.saveAs.current"] = "現在：",
            ["dialog.saveAs.cancel"] = "キャンセル",
            ["dialog.saveAs.save"] = "保存",

            ["dialog.unsaved.title"] = "未保存の変更",
            ["dialog.unsaved.message"] = "プロジェクト{0}に未保存の変更があります。閉じる前に保存しますか？",
            ["dialog.unsaved.cancel"] = "キャンセル",
            ["dialog.unsaved.discard"] = "破棄",
            ["dialog.unsaved.save"] = "保存",

            ["dialog.open.title"] = "プロジェクトファイルを開く",
            ["dialog.open.empty"] = "ドライブを選択するかパスを入力してください",
            ["dialog.open.emptyFolder"] = "空のフォルダーまたはアクセス拒否",
            ["dialog.open.cancel"] = "キャンセル",
            ["dialog.open.open"] = "開く",

            ["dialog.unlock.title"] = "プロジェクトのロック解除",
            ["dialog.unlock.message"] = "このプロジェクトはパスワードで保護されています。パスワードを入力してください。",
            ["dialog.unlock.password"] = "パスワード",
            ["dialog.unlock.cancel"] = "キャンセル",
            ["dialog.unlock.unlock"] = "ロック解除",

            ["dialog.setPw.title.change"] = "プロジェクトパスワードの変更",
            ["dialog.setPw.title.set"] = "プロジェクトパスワードの設定",
            ["dialog.setPw.msgChange"] = "プロジェクト保護パスワードを変更または削除します。",
            ["dialog.setPw.msgSet"] = "このプロジェクトを保護するパスワードを設定します。",
            ["dialog.setPw.currentPw"] = "現在のパスワード",
            ["dialog.setPw.newPw"] = "新しいパスワード",
            ["dialog.setPw.confirmPw"] = "パスワードの確認",
            ["dialog.setPw.cancel"] = "キャンセル",
            ["dialog.setPw.change"] = "パスワード変更",
            ["dialog.setPw.set"] = "パスワード設定",

            ["dialog.changePw.title"] = "パスワード変更",
            ["dialog.changePw.message"] = "続行するにはパスワードを変更する必要があります。",
            ["dialog.changePw.currentPw"] = "現在のパスワード",
            ["dialog.changePw.newPw"] = "新しいパスワード",
            ["dialog.changePw.confirmPw"] = "新しいパスワードの確認",
            ["dialog.changePw.submit"] = "パスワード変更",

            ["general.serverEditor"] = "サーバーエディター",
            ["general.projectLocked"] = "プロジェクトはロックされています。先にロックを解除してください。",
            ["general.projectUnlocked"] = "プロジェクトのロックが解除されました。",
            ["general.projectLocked2"] = "プロジェクトがロックされました。",
        },

        ["zh"] = new()
        {
            ["menu.file"] = "文件",
            ["menu.edit"] = "编辑",
            ["menu.view"] = "视图",
            ["menu.settings"] = "设置",
            ["menu.help"] = "帮助",

            ["menu.file.new"] = "新建",
            ["menu.file.open"] = "打开",
            ["menu.file.browseServer"] = "浏览服务器…",
            ["menu.file.recent"] = "最近使用",
            ["menu.file.save"] = "保存",
            ["menu.file.saveAs"] = "另存为…",

            ["menu.edit.addFolder"] = "添加文件夹",
            ["menu.edit.addVariable"] = "添加变量",
            ["menu.edit.addScript"] = "添加脚本",
            ["menu.edit.addScreen"] = "添加画面",
            ["menu.edit.copy"] = "复制",
            ["menu.edit.paste"] = "粘贴",
            ["menu.edit.delete"] = "删除",

            ["menu.view.panels"] = "面板",
            ["menu.view.maximize"] = "最大化活动面板",

            ["menu.settings.theme"] = "主题",
            ["menu.settings.light"] = "浅色",
            ["menu.settings.dark"] = "深色",
            ["menu.settings.language"] = "语言",
            ["menu.settings.protection"] = "项目保护",
            ["menu.settings.unlock"] = "解锁项目…",
            ["menu.settings.lock"] = "锁定项目",
            ["menu.settings.changePw"] = "更改密码…",
            ["menu.settings.removePw"] = "删除密码…",
            ["menu.settings.setPw"] = "设置密码…",

            ["menu.help.help"] = "帮助",
            ["menu.help.validate"] = "验证项目",
            ["menu.help.about"] = "关于",

            ["toolbar.new"] = "新建",
            ["toolbar.save"] = "保存",
            ["toolbar.saveAs"] = "另存为…",
            ["toolbar.undo"] = "撤销",
            ["toolbar.redo"] = "重做",
            ["toolbar.copy"] = "复制",
            ["toolbar.paste"] = "粘贴",
            ["toolbar.delete"] = "删除",
            ["toolbar.addFolder"] = "添加文件夹",
            ["toolbar.addVariable"] = "添加变量",
            ["toolbar.addScript"] = "添加脚本",
            ["toolbar.addPlc"] = "添加PLC程序",
            ["toolbar.addRecipe"] = "添加配方",
            ["toolbar.addScheduler"] = "添加调度器",
            ["toolbar.addReport"] = "添加报表",
            ["toolbar.addScreen"] = "添加画面",
            ["toolbar.restore"] = "还原布局",
            ["toolbar.maximize"] = "最大化活动面板",
            ["toolbar.toggleTheme"] = "切换主题",
            ["toolbar.logout"] = "注销",

            ["status.unsaved"] = "未保存",
            ["status.locked"] = "已锁定",
            ["status.unlocked"] = "已解锁",

            ["dialog.saveAs.title"] = "另存为",
            ["dialog.saveAs.filePath"] = "文件路径：",
            ["dialog.saveAs.current"] = "当前：",
            ["dialog.saveAs.cancel"] = "取消",
            ["dialog.saveAs.save"] = "保存",

            ["dialog.unsaved.title"] = "未保存的更改",
            ["dialog.unsaved.message"] = "项目{0}有未保存的更改。关闭前是否保存？",
            ["dialog.unsaved.cancel"] = "取消",
            ["dialog.unsaved.discard"] = "放弃",
            ["dialog.unsaved.save"] = "保存",

            ["dialog.open.title"] = "打开项目文件",
            ["dialog.open.empty"] = "选择驱动器或输入路径",
            ["dialog.open.emptyFolder"] = "空文件夹或访问被拒绝",
            ["dialog.open.cancel"] = "取消",
            ["dialog.open.open"] = "打开",

            ["dialog.unlock.title"] = "解锁项目",
            ["dialog.unlock.message"] = "此项目受密码保护。请输入密码以进行编辑。",
            ["dialog.unlock.password"] = "密码",
            ["dialog.unlock.cancel"] = "取消",
            ["dialog.unlock.unlock"] = "解锁",

            ["dialog.setPw.title.change"] = "更改项目密码",
            ["dialog.setPw.title.set"] = "设置项目密码",
            ["dialog.setPw.msgChange"] = "更改或删除项目保护密码。",
            ["dialog.setPw.msgSet"] = "设置密码以保护此项目。",
            ["dialog.setPw.currentPw"] = "当前密码",
            ["dialog.setPw.newPw"] = "新密码",
            ["dialog.setPw.confirmPw"] = "确认密码",
            ["dialog.setPw.cancel"] = "取消",
            ["dialog.setPw.change"] = "更改密码",
            ["dialog.setPw.set"] = "设置密码",

            ["dialog.changePw.title"] = "更改密码",
            ["dialog.changePw.message"] = "您必须在继续之前更改密码。",
            ["dialog.changePw.currentPw"] = "当前密码",
            ["dialog.changePw.newPw"] = "新密码",
            ["dialog.changePw.confirmPw"] = "确认新密码",
            ["dialog.changePw.submit"] = "更改密码",

            ["general.serverEditor"] = "服务器编辑器",
            ["general.projectLocked"] = "项目已锁定。请先解锁才能进行更改。",
            ["general.projectUnlocked"] = "项目已解锁。",
            ["general.projectLocked2"] = "项目已锁定。",
        },
    };
}
