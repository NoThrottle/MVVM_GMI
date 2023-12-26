namespace MVVM_GMI.Helpers
{
    public static class Extensions
    {
        public static IEnumerable<IEnumerable<T>> Split<T>(this T[] arr, int size)
        {
            for (var i = 0; i < arr.Length / size + 1; i++)
            {
                yield return arr.Skip(i * size).Take(size);
            }
        }
    }
}
