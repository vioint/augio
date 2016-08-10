using System;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;

namespace Augio
{
	public static class TypeExtensions
	{
		public static T GetAttribute<T> (this Enum value) where T : Attribute
		{

			T attribute;
			MemberInfo info = value.GetType ().GetMember (value.ToString ()).FirstOrDefault ();
			if (info != null) {
				attribute = (T)info.GetCustomAttributes (typeof(T), false).FirstOrDefault ();
				return attribute;
			}
			return null;
		}

		public static string GetDescription (this Enum value)
		{

			DescriptionAttribute attribute;
			MemberInfo info = value.GetType ().GetMember (value.ToString ()).FirstOrDefault ();
			if (info != null) {
				attribute = (DescriptionAttribute)info.GetCustomAttributes (typeof(DescriptionAttribute), false).FirstOrDefault ();
				return attribute != null ? attribute.Description : null;
			}
			return null;
		}

		public static T DescriptionToEnum<T> (this string description) where T : struct, IConvertible
		{
			return typeof(T).ToList<T> ().FirstOrDefault (e => (e as Enum).GetDescription () == description);
		}

		public static List<T> ToList<T> (this Type enumType) where T : struct, IConvertible
		{
			if (!typeof(T).IsEnum)
				throw new ArgumentException ("T must be an enumerated type");
		
			return Enum.GetValues (typeof(T))
			.OfType<T> ()
			.ToList ();
		}

		public static float Remap (this float value, float from1, float to1, float from2, float to2)
		{
			return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
		}
	}

}