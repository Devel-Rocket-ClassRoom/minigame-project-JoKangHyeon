using Cysharp.Threading.Tasks;

public interface IInitCheckable
{
    public bool IsInited();
    public UniTask<bool> Init();
}
