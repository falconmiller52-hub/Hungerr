using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Common.Extensions
{
	public static class ListExtensions
	{
		/// <summary>
		/// Возвращает случайный элемент из списка. 
		/// Если список пуст, выводит ошибку и возвращает значение по умолчанию.
		/// </summary>
		public static T Random<T>(this IList<T> list)
		{
			if (list == null || list.Count == 0)
			{
				Debug.LogError("ListExtensions::RandomElement() - Попытка получить элемент из пустого или null списка!");
				return default;
			}

			int randomIndex = UnityEngine.Random.Range(0, list.Count);
			return list[randomIndex];
		}
		
		/// <summary>
		/// Выбирает случайный элемент из списка, исключая указанный объект.
		/// Полезно для предотвращения повторения одного и того же звука или реплики дважды подряд.
		/// </summary>
		/// <typeparam name="T">Тип элементов в списке.</typeparam>
		/// <param name="list">Исходный список или массив.</param>
		/// <param name="except">Элемент, который НЕ должен быть выбран (например, последний проигранный звук).</param>
		/// <returns>Случайный элемент, отличный от исключаемого. Если в списке всего 1 элемент, вернет его.</returns>
		/// <remarks>
		/// Внимание: если список пуст, метод вернет значение по умолчанию (null для ссылочных типов).
		/// Если список содержит только один элемент, проверка на исключение будет проигнорирована во избежание бесконечного цикла.
		/// </remarks>
		public static T RandomExcept<T>(this IList<T> list, T except)
		{
			if (list == null || list.Count == 0) return default;
    
			// Если в списке всего один элемент, выбора нет — возвращаем его
			if (list.Count == 1) return list[0];

			T result;
			do
			{
				int randomIndex = UnityEngine.Random.Range(0, list.Count);
				result = list[randomIndex];
			} 
			// Перебираем, пока не найдем элемент, не равный предыдущему
			while (EqualityComparer<T>.Default.Equals(result, except));

			return result;
		}
	}
}