// ******************************************************************
//       /\ /|       @file       BaseSingleTon.cs
//       \ V/        @brief      单例基类
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2021-05-06 19:17:31
//    *(__\_\        @Copyright  Copyright (c) 2021, Shadowrabbit
// ******************************************************************

public class BaseSingleTon<T> where T : class, new()
{
    public static T Instance => Inner.InternalInstance;

    private static class Inner
    {
        internal static readonly T InternalInstance = new T();
    }
}