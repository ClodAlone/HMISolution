using System;

namespace UFRecipeExecuter
{
    public enum RecipeExecutionStateEnum : uint
    {
        None = 0x00000000,

        StateMaskFlag = 0x00000FFF,
        Initialization = 0x00000001,
        Finalization = 0x00000002,
        LoadingValues = 0x0000004,
        SavingValues = 0x0000008,
        DeletingValues = 0x0000010,
        ReadingValues = 0x0000020,
        WritingValues = 0x0000040,
        ImportingValues = 0x0000080,
        ExportingValues = 0x0000100,

        ErrorMaskFlag = 0x00FFF000,
        ErrorOnUpdatingList = 0x00001000,
        ErrorOnUpdatingState = 0x00002000,
        ErrorOnLoadingValues = 0x00004000,
        ErrorOnSavingValues = 0x00008000,
        ErrorOnDeletingValues = 0x00010000,
        ErrorOnReadingValues = 0x00020000,
        ErrorOnWritingValues = 0x00040000,
        ErrorOnImportingValues = 0x00080000,
        ErrorOnExportingValues = 0x00100000,
        TimeoutOnConnectCommandTags = 0x00200000,
        TimeoutOnConnectValueTags = 0x00400000,
        NotFoundOnLoadingValues = 0x00800000,

        SuccessfullMaskFlag = 0xFF000000,
        SuccessfullSaveValues = 0x02000000,
        SuccessfullDeleteValues = 0x04000000,
        SuccessfullReadValues = 0x08000000,
        SuccessfullWriteValues = 0x10000000,
        SuccessfullImportValues = 0x20000000,
        SuccessfullExportValues = 0x40000000,
        SuccessfullLoadingValues = 0x80000000
    }
}
