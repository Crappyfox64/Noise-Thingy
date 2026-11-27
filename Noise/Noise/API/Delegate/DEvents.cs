using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace CRPY.Events;

// ================== [ S I N G L E   V A L U E   E V E N T ]
public class CDEvent<T> : ICE
{
    protected event Action<T>? Del;

    public string Connect(Action<T> Method)
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

    public void Invoke(T Value)
    {
        Del?.Invoke(Value);
    }
}


// ================== [ M U L T I - S T A T E   E V E N T ]
public class CDSEvent<T> : ICE
{
    protected Dictionary<string,Action<T>> Del = new Dictionary<string,Action<T>>();
    public string Connect(string State, Action<T> Method)
    {
        string Key = GenKey();
        (string State,Action<T> Method) V = (State,Method);
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

    public void Invoke(string State,T Value)
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