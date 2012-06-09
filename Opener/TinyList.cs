using System;

namespace FileOpener.Opener
{
    public sealed class TinyList<T>
    {
        public readonly T Head;
        public readonly TinyList<T> Tail;

        private TinyList(T head, TinyList<T> tail)
        {
            Head = head;
            Tail = tail;
        }

        public static TinyList<T> Cons(T head, TinyList<T> tail)
        {
            return new TinyList<T>(head, tail);
        }
    }

    public static class TinyList
    {
        public static B Foldl<A, B>(this TinyList<A> list, B seed, Func<B, A, B> func)
        {
            while (list != null) {
                seed = func(seed, list.Head);
                list = list.Tail;
            }
            return seed;
        }

        public static B Foldr<A, B>(this TinyList<A> list, B seed, Func<A, B, B> func)
        {
            if (list == null) {
                return seed;
            }
            // reverse the list
            var reversed = Cons(list.Head, null);
            list = list.Tail;
            while (list != null) {
                reversed = Cons(list.Head, reversed);
                list = list.Tail;
            }
            // actual fold
            while (reversed != null) {
                seed = func(reversed.Head, seed);
                reversed = reversed.Tail;
            }
            return seed;
        }

        public static TinyList<T> Cons<T>(T head, TinyList<T> tail)
        {
            return TinyList<T>.Cons(head, tail);
        }

        public static TinyList<T> Cons<T>(this TinyList<T> tail, T head)
        {
            return TinyList<T>.Cons(head, tail);
        }
    }
}
