using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReader.SchemaInfo
{
    public enum DataSchemaType
    {
        // Summary:
        //     Configure the System.Data.DataSet without using the incoming schema.
        None = 0,
        // Summary:
        //     Ignore any table mappings on the DataAdapter. Configure the System.Data.DataSet
        //     using the incoming schema without applying any transformations.
        Source = 1,
        //
        // Summary:
        //     Apply any existing table mappings to the incoming schema. Configure the System.Data.DataSet
        //     with the transformed schema.
        Mapped = 2,
        // Summary:
        //     Ignore any table mappings on the DataAdapter. Configure the System.Data.DataSet
        //     using the incoming schema without applying any transformations.
        //     Read only schema information from source table.
        SourceOnlySchema = 3,
        //
        // Summary:
        //     Apply any existing table mappings to the incoming schema. Configure the System.Data.DataSet
        //     with the transformed schema.
        //     Read only schema information from source table.
        MappedOnlySchema = 4
    }
}
