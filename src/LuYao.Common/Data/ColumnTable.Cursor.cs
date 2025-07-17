namespace LuYao.Data;

partial class ColumnTable
{
    /// <summary>
    /// 当前游标位置
    /// </summary>
    public int Cursor
    {
        get => _currentRow;
        set => _currentRow = value;
    }

    private int _currentRow = -1;
    /// <summary>
    /// 读取下一行
    /// </summary>
    /// <returns></returns>
    public bool Read()
    {
        if (_currentRow < Count - 1)
        {
            _currentRow++;
            return true;
        }
        return false;
    }

    /// <summary>
    /// 重置当前游标位置
    /// </summary>
    public void Reset() => _currentRow = -1;
}
