using System.Collections;

namespace BoardGameTracker.UnitTests;

public abstract class TheoryData : IEnumerable<object[]>
{
    readonly List<object[]> data = [];

    protected void AddRow(params object[] values)
    {
        data.Add(values);
    }

    public IEnumerator<object[]> GetEnumerator()
    {
        return data.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public class TheoryData<T1, T2> : TheoryData
{
    public void Add(T1 p1, T2 p2)
    {
        AddRow(p1, p2);
    }
}
