// Class for storing a list of hittable objects.

using System.Runtime.CompilerServices;
using RTOneWeekend.Core;

namespace RTOneWeekend.Geometry;

public class HittableList : Hittable
{
	public List<Hittable> Objects {get;} = [];

	public HittableList() {}
	public HittableList(Hittable obj)
	{
		Add(obj);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Clear() => Objects.Clear();

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Add(Hittable obj) => Objects.Add(obj);

	public override bool Hit(in Ray r, Interval rayT, ref HitRecord rec)
	{
		HitRecord tempRec = default;
		bool hitAnything = false;
		double closestSoFar = rayT.Max;
		
		foreach(Hittable obj in Objects)
		{
			if (obj.Hit(r, new Interval(rayT.Min, closestSoFar), ref tempRec))
			{
				hitAnything = true;
				closestSoFar = tempRec.RayHitDistance;
				rec = tempRec;
			}
		}

		return hitAnything;
	}
}