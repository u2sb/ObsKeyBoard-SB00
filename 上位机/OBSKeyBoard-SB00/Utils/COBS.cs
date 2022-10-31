// https://github.com/LudwigThePig/COBS-in-C-Sharp/blob/master/COBS.cs

namespace OBSKeyBoard_SB00.Utils;

public static class COBS
{
    public static byte[] Encode(byte[] message, bool zeroFrame = false)
    {
        var result = new List<byte> { 0x00 };
        var codePointer = 0;
        byte code = 0x01;

        void Finish()
        {
            result[codePointer] = code;
            codePointer = result.Count;
            result.Add(0x00);
            code = 0x01;
        }

        for (var i = 0; i < message.Length; i++)
            if (code == 0xFF)
            {
                i--;
                Finish();
            }
            else if (message[i] == 0x00)
            {
                Finish();
            }
            else
            {
                result.Add(message[i]);
                code++;
            }

        result[codePointer] = code;

        if (zeroFrame)
            result.Add(0x00);

        var res = result.ToArray();
        var log = "";
        foreach (var t in res)
            log += t + ", ";

        Console.WriteLine(log);

        return result.ToArray();
    }


    public static byte[] Decode(byte[] message)
    {
        var input = message;
        var result = new List<byte>();
        var distanceIndex = 0;
        byte distance; // Distance to next zero

        while (distanceIndex < input.Length)
        {
            distance = input[distanceIndex];

            if (input.Length < distanceIndex + distance || distance < 1)
            {
                Console.WriteLine("Uh Oh! This is not a valid COBS message");
                return new byte[0];
            }

            if (distance > 1)
                for (byte i = 1; i < distance; i++)
                    result.Add(input[distanceIndex + i]);


            distanceIndex += distance;

            if (distance < 0xFF && distanceIndex < input.Length)
                result.Add(0);
        }

        return result.ToArray();
    }
}