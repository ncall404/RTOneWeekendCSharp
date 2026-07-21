// Class for representing a node in a bounding volume hierarchy (BVH).

using RTOneWeekend.Core;

namespace RTOneWeekend.Geometry;

public class BvhNode : Hittable
{
	private Hittable Left;
	private Hittable Right;
	public override Aabb BoundingBox { get; protected set; }

	// Copies the input list of objects into a new list and creates the BVH.
	public BvhNode(HittableList list): this(list.Objects, 0, list.Objects.Count) {}

	public BvhNode(List<Hittable> objects, int start, int end)
	{
		// NOTE: Sorting had to be done slightly differently to the tutorial due to language/library differences between C++ and C#.

		int axis = RandomNum.RandomInt(0, 2);

		// Do a comparison but get ints instead of bools so that the sort function can be used.
		Comparison<Hittable> comparator = (axis == 0) ? BoxXCompare : (axis == 1) ? BoxYCompare : BoxZCompare;

		int objectSpan = end - start;

		if (objectSpan == 1)
		{
			Left = Right = objects[start];
		}
		else if (objectSpan == 2)
		{
			Left = objects[start];
			Right = objects[start + 1];
		}
		else
		{
			objects.Sort(start, objectSpan, Comparer<Hittable>.Create(comparator)); // Create a comparer to act the same as std::sort in the tutorial.
			int mid = start + objectSpan / 2;
			Left = new BvhNode(objects, start, mid);
			Right = new BvhNode(objects, mid, end);
		}

		BoundingBox = new Aabb(Left.BoundingBox, Right.BoundingBox);
	}

	public override bool Hit(in Ray r, Interval rayT, ref HitRecord rec)
	{
		if (!BoundingBox.Hit(r, rayT))
			return false;

		bool HitLeft = Left.Hit(r, rayT, ref rec);
		bool HitRight = Right.Hit(r, HitLeft ? new Interval(rayT.Min, rec.RayHitDistance) : rayT, ref rec);

		return HitLeft || HitRight;
	}

	private static int BoxCompare(Hittable a, Hittable b, int axisIndex)
	{
		Interval aAxisInterval = a.BoundingBox.AxisInterval(axisIndex);
		Interval bAxisInterval = b.BoundingBox.AxisInterval(axisIndex);

		if (aAxisInterval.Min < bAxisInterval.Min) return -1;
		if (aAxisInterval.Min > bAxisInterval.Min) return 1;
		return 0;
	}
	private static int BoxXCompare(Hittable a, Hittable b) => BoxCompare(a, b, 0);
	private static int BoxYCompare(Hittable a, Hittable b) => BoxCompare(a, b, 1);
	private static int BoxZCompare(Hittable a, Hittable b) => BoxCompare(a, b, 2);
}