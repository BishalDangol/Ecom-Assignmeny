using System;
using System.Security.Cryptography;
using System.Text;

public static class EsewaUtil
{
    private const string SecretKey = "8gBm/:&EnhH.1/q"; // TEST SECRET KEY for eSewa v2

    public static string GenerateSignature(string totalAmount, string transactionUuid, string productCode)
    {
        // Format: total_amount=X,transaction_uuid=Y,product_code=Z
        string data = string.Format("total_amount={0},transaction_uuid={1},product_code={2}", 
                                    totalAmount, transactionUuid, productCode);
        
        return HashHmacSha256(data, SecretKey);
    }

    private static string HashHmacSha256(string message, string secret)
    {
        var encoding = new UTF8Encoding();
        byte[] keyByte = encoding.GetBytes(secret);
        byte[] messageBytes = encoding.GetBytes(message);
        using (var hmacsha256 = new HMACSHA256(keyByte))
        {
            byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
            return Convert.ToBase64String(hashmessage);
        }
    }
}
