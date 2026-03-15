namespace OPCUAViewModel.Services
{
    public enum OpcClientErrorCodes
    {
        OpcClientErrorCodeBadConnetion = 100,
        OpcClientErrorCodeBadWaitingForInitialData = 101,
        OpcClientErrorReadWriteFailed = 102,
        OpcClientErrorWritingUnexpectedError = 103,
        OpcClientErrorBadNodeIdOrInvalidState = 104
    }
}
