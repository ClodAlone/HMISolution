using System.Security.Cryptography.X509Certificates;

namespace OpcUaUnitTests;

public abstract class BaseUnitTest
{
    protected X509Certificate2 GetRandomCertificate()
    {
        var st = new X509Store(StoreName.My, StoreLocation.LocalMachine);
        st.Open(OpenFlags.ReadOnly);
        try
        {
            var certCollection = st.Certificates;
            return certCollection.Count == 0 ? null : certCollection[0];
        }
        finally
        {
            st.Close();
        }
    }
}