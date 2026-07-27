// Class for representing a node in a bounding volume hierarchy (BVH).

using RTOneWeekend.Core;

namespace RTOneWeekend.Geometry;

public class BvhNode : Hittable
{
	private readonly Hittable _left;
	private readonly Hittable _right;
	public override Aabb BoundingBox { get; protected set; }

	// Copies the input list of objects into a new list and creates the BVH.
	public BvhNode(HittableList list): this(list.Objects, 0, list.Objects.Count) {}

	public BvhNode(List<Hittable> objects, int start, int end)
	{
		// Build the bounding box of the span of source objects.
		BoundingBox = Aabb.Empty;
		for (int objectIndex = start; objectIndex < end; objectIndex++)
			BoundingBox = new Aabb(BoundingBox, objects[objectIndex].BoundingBox);

		int axis = RandomNum.RandomInt(0, 2);

		// Do a comparison but get ints instead of bools (which the tutorial uses) so that the List sort function can be used.
		Comparison<Hittable> comparator = (axis == 0) ? BoxXCompare : (axis == 1) ? BoxYCompare : BoxZCompare;

		int objectSpan = end - start;

		if (objectSpan == 1)
		{
			_left = _right = objects[start];
		}
		else if (objectSpan == 2)
		{
			_left = objects[start];
			_right = objects[start + 1];
		}
		else
		{
			// NOTE: Sorting had to be done slightly differently to the tutorial due to language/library differences between C++ and C#.
			objects.Sort(start, objectSpan, Comparer<Hittable>.Create(comparator)); // Create a comparer to act the same as std::sort in the tutorial.
			int mid = start + objectSpan / 2;
			_left = new BvhNode(objects, start, mid);
			_right = new BvhNode(objects, mid, end);
		}
	}

	public override bool Hit(in Ray r, Interval rayT, ref HitRecord rec)
	{
		if (!BoundingBox.Hit(r, rayT))
			return false;

		bool HitLeft = _left.Hit(r, rayT, ref rec);
		bool HitRight = _right.Hit(r, HitLeft ? new Interval(rayT.min, rec.rayHitDistance) : rayT, ref rec);

		return HitLeft || HitRight;
	}

	private static int BoxCompare(Hittable a, Hittable b, int axisIndex)
	{
		Interval aAxisInterval = a.BoundingBox.AxisInterval(axisIndex);
		Interval bAxisInterval = b.BoundingBox.AxisInterval(axisIndex);

		if (aAxisInterval.min < bAxisInterval.min) return -1;
		if (aAxisInterval.min > bAxisInterval.min) return 1;
		return 0;
	}
	private static int BoxXCompare(Hittable a, Hittable b) => BoxCompare(a, b, 0);
	private static int BoxYCompare(Hittable a, Hittable b) => BoxCompare(a, b, 1);
	private static int BoxZCompare(Hittable a, Hittable b) => BoxCompare(a, b, 2);
}