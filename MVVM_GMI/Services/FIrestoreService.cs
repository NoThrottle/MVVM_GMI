using Google.Cloud.Firestore;
using System.IO;

namespace MVVM_GMI.Services
{
    internal static class FirestoreService
    {

        static string fireConfig = @"
        {
          ""type"": ""service_account"",
          ""project_id"": ""hsmclauncher"",
          ""private_key_id"": ""593e0b9e778c38a764eab12eaa9e294205e274dd"",
          ""private_key"": ""-----BEGIN PRIVATE KEY-----\nMIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQDOAEYsU9fgUJ5Y\nMbqc4Jh/KWAEof+ONqrGLulWBF0Ia130IhCaIGVFYtOXUXS3cLLJ4h42BA+11Lxc\nZ2nWiaafLOzJRohD4AyNP2PC315wVTZrfRbNlp5oVp7IxefpsOuVTtdzMOjJgPdj\nkzCKAVykG3EB9URKLWqSsNeyWn3w0cUQgzgdd9KF7u8Vn8TtDmEr446nSyYdFfxx\nd5O/E+x146/aVPHOEKscJZ+lBQOFhP7gLsKybzOOoVInRenlcgXTGOtsK2ETgoFh\n/di7Cw1TeMcg/Rl9/aZ4tRqQPGcZww0trZOVHzpELOikQgUEsD/tX6J0wtHda5bE\nd/B9OmEnAgMBAAECggEAAf/Gro9xvN+Ladexs+wKDtHb+K+Jx1uIdLvnz8njDT3d\nf40FFDnUQcQfwYws0crqMUNcf7GyZVVXLp5iTI6xa0IIWjuw/tehx40h7rY0OGrb\nSFfCbORVTTXp/JDPG1l4L87AEIIdADecLv8yrWQt/fm2MwqqSv391saYkUaHilq6\nVPkWA5EtCp6CyLPvVPLc0ThkTTGwwFQCaJtHJHhV7s1tTAlyUg1X8HdDLepnCYzM\nblt1u4k8tkXrGnIeTs+Wt5stC6/QF3oJBNjPEkh7hhUcSLO5Ji+5fOS6K5on1aYE\nl2HzzPposbDQqhf8dJK8E3Syi+4AkTbZgz2bI++dIQKBgQDunmiYAmb1IcZfsMVI\np1P/JfYTELpnT4ASk3VyukdbbjMnUJiEBjhapaHdzqUt9/gFRnXdw0T1ocgMga7S\nApglXMTfTKSaG0T3QuOXXUdOWbRFLa2QhA/FBjynnj43H1dh6HhKTHrTShTg0Aro\nxOEcoaGQiQ+DYVeK2g1EAI3uPQKBgQDdAaJHShVzNaWUimaMBHE4IM/jmpgDOSdT\n4ijfG4zOjzFMYGFRAFUY/hTOPmRH4vM4u5PToK/G+kfP9zYvqbxH1I9beVgssIqf\ngJSnKmwTRsua+1uroQYpprYCG7kucm380d6r7ZbZQFRD1yxshvpZKAJ/Jw1FI8Jy\nHksfynVHMwKBgHKLkGIIJSxfPj8JOGscB/JfYrTcPt8BBtXj+2lLip2VVPD2e4BF\nDCBXilBTtm9Orby6ijIeXqgbNVDrn+QoYqjs04Tt4cYoeP7JGT+8A1LVNPhQaRUK\nkBdgWxWLnQoQ/X1+fuALvppW4ZbZx6Ldf8KixFABu3Sx8bgx3FRcf8VxAoGBAI0e\nOPeaV338OGFTyk0HnR/A292yXfRY70LBu2VS2yMeDdV+CUXGl4/g1XooHrJEORf1\ntlvAU90S/P3PFZtzn1SiZnN7VREcHTfhy4m+LcpVPzcT3yIFLME1wSwxdZox2QdB\nHxTbHaJVUDkNUUFvIrYBNS7pybBnOAbUTxXame4/AoGASLxUbRk/xL6Aoo30DX1o\nWo7FrAiHWlymsuSMz/CmTRXHdByYSwmH5H3zL6ktoFHjYt8Tn/wscvRQBnMLCyir\nOTI5FevYA5ftbWHJMyCjlEsFgeadvZ4USeo9NI6D4mvmdTbZrtdACEa71rNKdmDA\n1VszBi8X5HwyCM6Mg15F7J8=\n-----END PRIVATE KEY-----\n"",
          ""client_email"": ""firebase-adminsdk-x22xd@hsmclauncher.iam.gserviceaccount.com"",
          ""client_id"": ""109528851856129425104"",
          ""auth_uri"": ""https://accounts.google.com/o/oauth2/auth"",
          ""token_uri"": ""https://oauth2.googleapis.com/token"",
          ""auth_provider_x509_cert_url"": ""https://www.googleapis.com/oauth2/v1/certs"",
          ""client_x509_cert_url"": ""https://www.googleapis.com/robot/v1/metadata/x509/firebase-adminsdk-x22xd%40hsmclauncher.iam.gserviceaccount.com"",
          ""universe_domain"": ""googleapiss.com""
        }" ;

        static string filePath = "";
        public static FirestoreDb Database {  get; private set; }

        public static void SetEnvironmentVariable()
        {
            filePath = Path.Combine(Path.GetTempPath(), Path.GetFileNameWithoutExtension(Path.GetRandomFileName())) + ".json";
            File.WriteAllText(filePath, fireConfig);
            File.SetAttributes(filePath, FileAttributes.Hidden);
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", filePath);
            Database = FirestoreDb.Create("hsmclauncher");
            File.Delete(filePath);


        }


    }
}
