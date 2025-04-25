namespace Framework;

public abstract class Pipe<TChain>
    where TChain : Pipe<TChain>
{
    public TChain Chain { get; private set; } = default!;
    public bool HasChain => Chain is not null;

    public void OnChain(TChain chain) => Chain = chain;
}

