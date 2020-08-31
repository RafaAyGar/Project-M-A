using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public enum ServerPackets
{
    Welcome,
    Disconnect,
}
public enum ClientPackets
{
    WelcomeReceived,
    Disconnect
}

public class Packet : IDisposable
{
    public byte[] contentArray = new byte[4096];
    private int readPos = 0;
    private int arrayBytesCount = 0;


    public Packet(byte[] content)
    {
        Write(content);
        readPos = 0;
    }

    public Packet(int packetType)
    {
        Write(packetType);
    }

    public byte[] ToArray()
    {
        return contentArray;
    }

    public int Length()
    {
        return arrayBytesCount;
    }

    public int UnreadLength()
    {
        return arrayBytesCount - readPos;
    }

    void WriteInArray(byte[] bytes)
    {
        bytes.CopyTo(contentArray, arrayBytesCount);
        arrayBytesCount += bytes.Length;
    }

    public void Write(byte[] bytes)
    {
        WriteInArray(bytes);
    }

    public void Write(int integer)
    {
        byte[] bytes = BitConverter.GetBytes(integer);
        WriteInArray(bytes);
    }

    public void Write(bool boolean)
    {
        byte[] bytes = BitConverter.GetBytes(boolean);
        WriteInArray(bytes);
    }

    public void Write(string text)
    {
        Write(text.Length);
        byte[] bytes = Encoding.ASCII.GetBytes(text);
        WriteInArray(bytes);
    }


    public int ReadInt()
    {
        if (arrayBytesCount < 4) throw new Exception("Couldn't read integer from packet");

        int integerRead = BitConverter.ToInt32(contentArray, readPos);
        readPos += 4;

        return integerRead;
    }

    public bool ReadBool()
    {
        if (arrayBytesCount < 4) throw new Exception("Couldn't read bool from packet");

        bool boolean = BitConverter.ToBoolean(contentArray, readPos);
        readPos += 4;

        return boolean;
    }

    public string ReadString()
    {
        try
        {
            int stringLength = ReadInt();
            string readString = Encoding.ASCII.GetString(contentArray, readPos, stringLength);
            if (readString.Length > 0) readPos += stringLength;
            return readString;
        }
        catch
        {
            throw new Exception("Couldn't read string from packet");
        }
    }

    #region IDisposable implementation
    private bool disposed = false;

    protected virtual void Dispose(bool _disposing)
    {
        if (!disposed)
        {
            if (_disposing)
            {
                contentArray = null;
                arrayBytesCount = 0;
                readPos = 0;
            }

            disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    #endregion
}
