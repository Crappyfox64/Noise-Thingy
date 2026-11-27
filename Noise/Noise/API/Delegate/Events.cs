using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace CRPY.Events;


// ================== [ M A I N   A B S T R A C T ]
public abstract class ICE
{
    public delegate void VACT();
    protected Dictionary<string,dynamic> IDs = new Dictionary<string, dynamic>();
    protected static (int Min,int Max) IDLength = (100000,999999);
    protected static string KeySign = "[ICE]:";
    protected static Random R = new Random();

    protected string GenKey()
    {
        string K;
        while (true)
        {
            K = $"{KeySign}[{R.Next(IDLength.Min,IDLength.Max)}]";
            if (!IDs.ContainsKey(K)) return K;
        }
    }

    public static CEvent Event => new CEvent();
    public static CSEvent StateEvent => new CSEvent();
}


// ================== [ S I N G L E   V A L U E   E V E N T ]
public class CEvent : ICE
{
    protected event Action<dynamic>? Del;

    public string Connect(Action<dynamic> Method)
    {
        string Key = GenKey();
        IDs.Add(Key,Method);
        Del += Method;
        return Key;
    }

    public void Disconnect(string Key)
    {
        if (IDs.ContainsKey(Key))
        {
            Del -= IDs[Key];
            IDs.Remove(Key);
        }
    }

    public void Invoke(dynamic Value)
    {
        Del?.Invoke(Value);
    }
}


// ================== [ M U L T I - S T A T E   E V E N T ]
public class CSEvent : ICE
{
    protected Dictionary<string,Action<dynamic>> Del = new Dictionary<string,Action<dynamic>>();
    public string Connect(string State, Action<dynamic> Method)
    {
        string Key = GenKey();
        (string State,Action<dynamic> Method) V = (State,Method);
        IDs.Add(Key,V);
        if (!Del.ContainsKey(State))
        {
            Del.Add(State,Method);
        }else
        {
            Del[State] += Method;
        }
        return Key;
    }
    

    public void Disconnect(string Key)
    {
        if (!IDs.ContainsKey(Key)) return;
        var State = IDs[Key];
        Del[State.State] -= IDs[Key].Method;
        IDs.Remove(Key);
    }

    public void Invoke(string State,dynamic Value)
    {
        if (State.StartsWith("?>"))
        {
            switch (State.TrimStart('?','>').ToLower())
            {
                case "global":
                    foreach ( var V in Del ) V.Value?.Invoke(Value);
                break;
            }
            return;
        }
        if (!Del.ContainsKey(State)) return;
        Del[State]?.Invoke(Value);
    }
}