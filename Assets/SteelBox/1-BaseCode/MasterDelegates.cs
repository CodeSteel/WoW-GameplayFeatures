namespace SteelBox
{
    public delegate void ActionValValValRef<T1, T2, T3, T4>(T1 arg1, T2 arg2, T3 arg3, ref T4 arg4);
    public delegate void ActionValValRef<T1, T2, T3>(T1 arg1, T2 arg2, ref T3 arg3);
    public delegate void ActionValRef<T1, T2>(T1 arg1, ref T2 arg2);
    public delegate void ActionRef<T1>(ref T1 arg1);
}
