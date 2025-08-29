namespace Base.Sender;

public readonly struct SendProperties(bool persistent = false)
{
    public bool Persistent => persistent;
}