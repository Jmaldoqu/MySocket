using System;
using System.Text.Json;
using System.Text;

public class JsonMessageSerializer : ISerializer
{
    public byte[] Serializate(Object message)
    {
        try
        {
            string jsonString = JsonSerializer.Serialize(message);
            byte[] dataBytes = Encoding.UTF8.GetBytes(jsonString);
            return dataBytes;
        }
        catch (JsonException e)
        {
            Console.WriteLine("JSON serialization error: " + e.ToString);
            return null;
        }
        catch (Exception e)
        {
            Console.WriteLine("Serialization error: " + e.ToString);
            return null;
        }
    }

    public Object Deserializate(byte[] data)
    {
        try
        {
            string jsonString = Encoding.UTF8.GetString(data).Trim('\0');
            return JsonSerializer.Deserialize<Object>(data);
        }
        catch (JsonException e)
        {
            Console.WriteLine("JSON deserialization error: " + e.ToString);
            return null;
        }
        catch (Exception e)
        {
            Console.WriteLine("Deserialization errori: " + e.ToString);
            return null;
        }
    }
}