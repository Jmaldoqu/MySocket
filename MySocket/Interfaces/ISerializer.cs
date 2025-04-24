using System;

interface ISerializer
{
    public byte[] Serializate(Object message);
    public Object Deserializate(byte[] data);
}


