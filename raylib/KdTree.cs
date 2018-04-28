// -----------------------------------------------------------------------
// <copyright file="KdTree.cs" company="ZubeNET">
//   Copyright...
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;

namespace raylib
{
  public enum KdNodePlane
  {
    NoPlane,
    XY,
    YZ,
    XZ,
  };

  public class KdNode
  {
    public KdNode(KdNodePlane plane, PosVector coord, List<IShape> list, KdNode nodeLeft, KdNode nodeRight)
    {
      Plane = plane;
      Coord = coord;
      Shapes = list;
      Left = nodeLeft;
      Right = nodeRight;
    }

    public KdNodePlane Plane { get; }
    public PosVector Coord { get; }
    public List<IShape> Shapes { get; }
    public KdNode Left { get; }
    public KdNode Right { get; }
  }

  public class KdTree
  {
    private KdTree(KdNode root, BoundingBox bbox)
    {
      Root = root;
      BoundingBox = bbox;
    }

    public KdNode Root { get; }
    public BoundingBox BoundingBox { get; }

    private static int MaxTreeDepth => 20;
    private static int NumObjectsInLeaf => 1;

    public static KdTree Create(IEnumerable<IShape> shapes)
    {
      var shapesList = shapes.ToList();

      var bbox = new BoundingBox();
      foreach (var shape in shapesList)
      {
        bbox = bbox.GetEnlargedToEnclose(shape.BoundingBox);
      }

      var root = RecursiveBuild(shapesList, bbox, 0);

      return new KdTree(root, bbox);
    }

    public IntersectionInfo FindIntersectionTree(Ray ray)
    {
      if (IsBoundingBoxIntersection(BoundingBox, ray))
      {
        return FindIntersectionNode(Root, BoundingBox, ray);
      }
      else
      {
        return new IntersectionInfo();
      }
    }

    private IntersectionInfo FindIntersectionNode(KdNode node, BoundingBox bbox, Ray ray)
    {
      if (node == null)
      {
        return new IntersectionInfo();
      }

      if (node.Plane == KdNodePlane.NoPlane)
      {
        // leaf node

        var bestInfo = new IntersectionInfo();

        foreach (var shape in node.Shapes)
        {
          var info = shape.Intersect(ray);
          if (info.IsHit)
          {
            // if (bbox.IsPointInside(info.Position))
            {
              if (info.Distance < bestInfo.Distance && info.Distance >= 0.0)
              {
                bestInfo = info;
              }
            }
          }
        }

        if (bestInfo.IsHit)
        {
          return bestInfo;
        }
        else
        {
          return new IntersectionInfo();
        }
      }

      var (leftIsFront, frontBbox, backBbox) = SplitPlane(node, bbox, ray);

      var frontNode = leftIsFront ? node.Left : node.Right;
      var backNode = leftIsFront ? node.Right : node.Left;

      if (IsBoundingBoxIntersection(frontBbox, ray))
      {
        var intInfo = FindIntersectionNode(frontNode, frontBbox, ray);
        if (intInfo.IsHit)
        {
          return intInfo;
        }
        else
        {
          if (IsBoundingBoxIntersection(backBbox, ray))
          {
            intInfo = FindIntersectionNode(backNode, backBbox, ray);
            return intInfo.IsHit ? intInfo : new IntersectionInfo();
          }
          else
          {
            return new IntersectionInfo();
          }
        }
      }
      else
      {
        return new IntersectionInfo();
      }
    }

    private (bool, BoundingBox, BoundingBox) SplitPlane(KdNode node, BoundingBox bbox, Ray ray)
    {
      (bool, BoundingBox, BoundingBox) ExplicitSplitPlane(bool match)
      {
        if (match)
        {
          var (fbox, backbox) = SplitBoundingBox(bbox, node.Plane, node.Coord);
          return (true, fbox, backbox);
        }
        else
        {
          var (backbox, fbox) = SplitBoundingBox(bbox, node.Plane, node.Coord);
          return (false, fbox, backbox);
        }
      }

      switch (node.Plane)
      {
      case KdNodePlane.XY:
        return ExplicitSplitPlane(
          ((node.Coord.Z > bbox.BoxMin.Z) && (node.Coord.Z > ray.Position.Z)) ||
          ((node.Coord.Z < bbox.BoxMin.Z) && (node.Coord.Z < ray.Position.Z)));
      case KdNodePlane.XZ:
        return ExplicitSplitPlane(
          ((node.Coord.Y > bbox.BoxMin.Y) && (node.Coord.Y > ray.Position.Y)) ||
          ((node.Coord.Y < bbox.BoxMin.Y) && (node.Coord.Y < ray.Position.Y)));
      case KdNodePlane.YZ:
        return ExplicitSplitPlane(
          ((node.Coord.X > bbox.BoxMin.X) && (node.Coord.X > ray.Position.X)) ||
          ((node.Coord.X < bbox.BoxMin.X) && (node.Coord.X < ray.Position.X)));
      default:
        throw new InvalidOperationException();
      }
    }

    private bool Within(double val, double min, double max)
    {
      return val > min && val < max;
    }

    private bool IsBoundingBoxIntersection(BoundingBox bbox, Ray ray)
    {
      if (bbox.IsPointInside(ray.Position))
      {
        return true;
      }

      PosVector p;

      p = GetVectorPlaneIntersection(ray, KdNodePlane.XY, new PosVector(0.0, 0.0, bbox.BoxMin.Z));
      if (p != null && bbox.IsWithinX(p.X) && bbox.IsWithinY(p.Y))
      {
        return true;
      }

      p = GetVectorPlaneIntersection(ray, KdNodePlane.XY, new PosVector(0.0, 0.0, bbox.BoxMax.Z));
      if (p != null && bbox.IsWithinX(p.X) && bbox.IsWithinY(p.Y))
      {
        return true;
      }

      p = GetVectorPlaneIntersection(ray, KdNodePlane.XZ, new PosVector(0.0, bbox.BoxMin.Y, 0.0));
      if (p != null && bbox.IsWithinX(p.X) && bbox.IsWithinZ(p.Z))
      {
        return true;
      }

      p = GetVectorPlaneIntersection(ray, KdNodePlane.XZ, new PosVector(0.0, bbox.BoxMax.Y, 0.0));
      if (p != null && bbox.IsWithinX(p.X) && bbox.IsWithinZ(p.Z))
      {
        return true;
      }

      p = GetVectorPlaneIntersection(ray, KdNodePlane.YZ, new PosVector(bbox.BoxMin.X, 0.0, 0.0));
      if (p != null && bbox.IsWithinY(p.Y) && bbox.IsWithinZ(p.Z))
      {
        return true;
      }

      p = GetVectorPlaneIntersection(ray, KdNodePlane.YZ, new PosVector(bbox.BoxMax.X, 0.0, 0.0));
      if (p != null && bbox.IsWithinY(p.Y) && bbox.IsWithinZ(p.Z))
      {
        return true;
      }

      return false;
    }

    private PosVector GetVectorPlaneIntersection(Ray ray, KdNodePlane plane, PosVector coord)
    {
      switch (plane)
      {
      case KdNodePlane.XY:
        if (((coord.Z < ray.Position.Z) && (ray.Direction.Z > 0.0)) ||
            ((coord.Z > ray.Position.Z) && (ray.Direction.Z < 0.0)))
        {
          return null;
        }
        else
        {
          double k = (coord.Z - ray.Position.Z) / ray.Direction.Z;
          return new PosVector(
            ray.Position.X + (ray.Direction.X * k),
            ray.Position.Y + (ray.Direction.Y * k),
            coord.Z);
        }
      case KdNodePlane.XZ:
        if (((coord.Y < ray.Position.Y) && (ray.Direction.Y > 0.0)) ||
            ((coord.Y > ray.Position.Y) && (ray.Direction.Y < 0.0)))
        {
          return null;
        }
        else
        {
          double k = (coord.Y - ray.Position.Y) / ray.Direction.Y;
          return new PosVector(
            ray.Position.X + (ray.Direction.X * k),
            coord.Y,
            ray.Position.Z + (ray.Direction.Z * k));
        }
      case KdNodePlane.YZ:
        if (((coord.X < ray.Position.X) && (ray.Direction.X > 0.0)) ||
            ((coord.X > ray.Position.X) && (ray.Direction.X < 0.0)))
        {
          return null;
        }
        else
        {
          double k = (coord.X - ray.Position.X) / ray.Direction.X;
          return new PosVector(
            coord.X,
            ray.Position.Y + (ray.Direction.Y * k),
            ray.Position.Z + (ray.Direction.Z * k));
        }
      default:
        throw new InvalidOperationException();
      }
    }

    private static KdNode RecursiveBuild(List<IShape> shapes, BoundingBox bbox, int depth)
    {
      var (plane, coord) = FindPlane(shapes, bbox, depth);

      if (plane == KdNodePlane.NoPlane)
      {
        return MakeLeaf(shapes);
      }
      else
      {
        var (bboxLeft, bboxRight) = SplitBoundingBox(bbox, plane, coord);
        var (shapesLeft, shapesRight) = FilterOverlappedObjects(shapes, bboxLeft, bboxRight);

        var nodeLeft = RecursiveBuild(shapesLeft, bboxLeft, depth + 1);
        var nodeRight = RecursiveBuild(shapesRight, bboxRight, depth + 1);

        return new KdNode(plane, coord, new List<IShape>(), nodeLeft, nodeRight);
      }
    }

    private static (List<IShape>, List<IShape>) FilterOverlappedObjects(
      List<IShape> shapes,
      BoundingBox bboxLeft,
      BoundingBox bboxRight)
    {
      var shapesLeft = new List<IShape>();
      var shapesRight = new List<IShape>();

      foreach (var shape in shapes)
      {
        if (IsShapeInBoundingBox(shape, bboxLeft))
        {
          shapesLeft.Add(shape);
        }
        else
        {
          shapesRight.Add(shape);
        }
      }

      return (shapesLeft, shapesRight);
    }

    private static bool IsShapeInBoundingBox(IShape shape, BoundingBox bbox)
    {
      var shapeMin = shape.BoundingBox.BoxMin;
      var shapeMax = shape.BoundingBox.BoxMax;

      var boxMin = bbox.BoxMin;
      var boxMax = bbox.BoxMax;

      return !((shapeMax.X < boxMin.X) || (shapeMax.Y < boxMin.Y) || (shapeMax.Z < boxMin.Z) ||
               (shapeMin.X > boxMax.X) || (shapeMin.Y > boxMax.Y) || (shapeMin.Z > boxMax.Z));
    }

    private static (BoundingBox, BoundingBox) SplitBoundingBox(
      BoundingBox bbox,
      KdNodePlane plane,
      PosVector coord)
    {
      BoundingBox bboxLeft;
      BoundingBox bboxRight;

      switch (plane)
      {
      case KdNodePlane.XY:
        bboxLeft = new BoundingBox(
          new Bound(bbox.BoxMin.X, bbox.BoxMax.X),
          new Bound(bbox.BoxMin.Y, bbox.BoxMax.Y),
          new Bound(bbox.BoxMin.Z, coord.Z));
        bboxRight = new BoundingBox(
          new Bound(bbox.BoxMin.X, bbox.BoxMax.X),
          new Bound(bbox.BoxMin.Y, bbox.BoxMax.Y),
          new Bound(coord.Z, bbox.BoxMax.Z));
        break;
      case KdNodePlane.XZ:
        bboxLeft = new BoundingBox(
          new Bound(bbox.BoxMin.X, bbox.BoxMax.X),
          new Bound(bbox.BoxMin.Y, coord.Y),
          new Bound(bbox.BoxMin.Z, bbox.BoxMax.Z));
        bboxRight = new BoundingBox(
          new Bound(bbox.BoxMin.X, bbox.BoxMax.X),
          new Bound(coord.Y, bbox.BoxMax.Y),
          new Bound(bbox.BoxMin.Z, bbox.BoxMax.Z));
        break;
      case KdNodePlane.YZ:
        bboxLeft = new BoundingBox(
          new Bound(bbox.BoxMin.X, coord.X),
          new Bound(bbox.BoxMin.Y, bbox.BoxMax.Y),
          new Bound(bbox.BoxMin.Z, bbox.BoxMax.Z));
        bboxRight = new BoundingBox(
          new Bound(coord.X, bbox.BoxMax.X),
          new Bound(bbox.BoxMin.Y, bbox.BoxMax.Y),
          new Bound(bbox.BoxMin.Z, bbox.BoxMax.Z));
        break;
      default:
        throw new InvalidOperationException();
      }

      return (bboxLeft, bboxRight);
    }

    private static KdNode MakeLeaf(List<IShape> shapes)
    {
      return new KdNode(KdNodePlane.NoPlane, PosVector.NewDefault(), shapes, null, null);
    }

    /*
    * Using Surface Area Heuristic (SAH) for finding best split pane
    * SAH = 0.5 * voxel_surface_area * number_of_objects_in_voxel
    * splitted_SAH = split_cost
    *                + 0.5 * left_voxel_surface_area * number_of_objects_in_left_voxel
    *                + 0.5 * right_voxel_surface_area * number_of_objects_in_right_voxel
    * Finding coordinate of split plane (XY, XZ or YZ) which minimizing SAH
    * If can't find optimal split plane - returns NONE
    * see: http://stackoverflow.com/a/4633332/653511
    */
    private static (KdNodePlane, PosVector) FindPlane(List<IShape> shapes, BoundingBox bbox, int depth)
    {
      var finalPlane = KdNodePlane.NoPlane;
      var finalPos = PosVector.NewDefault();

      if (depth >= MaxTreeDepth || shapes.Count <= NumObjectsInLeaf)
      {
        return (KdNodePlane.NoPlane, PosVector.NewDefault());
      }

      double hx = bbox.BoxMax.X - bbox.BoxMin.X;
      double hy = bbox.BoxMax.Y - bbox.BoxMin.Y;
      double hz = bbox.BoxMax.Z - bbox.BoxMin.Z;

      // calculate square of each side of initial bounding box
      double sxy = hx * hy;
      double sxz = hx * hz;
      double syz = hy * hz;

      double ssum = sxy + sxz + syz;

      // normalize square of each side of initial bounding box to satisfy following relationship:
      // sxy + sxz + syz = 1
      sxy = sxy / ssum;
      sxz = sxz / ssum;
      syz = syz / ssum;

      int maxSplits = 5; // max splits of bounding box
      double splitCost = 5.0;

      // assum that at beginning best SAH has initial bounding box
      // SAH = 0.5 * square * objectsCount
      // square of initial bounding box is sxy + sxz + syz = 1
      double bestSah = Convert.ToDouble(shapes.Count);
      // initial bounding box doesn't have split plane
      finalPlane = KdNodePlane.NoPlane;

      var currentSplitCoord = PosVector.NewDefault();

      // find split surface which have the least SAH

      // trying to minimize SAH by splitting across XY plane
      double sSplit = sxy;
      double sNonSplit = sxz + syz;
      for (int i = 1; i < maxSplits; i++)
      {
        double l = Convert.ToDouble(i) / Convert.ToDouble(maxSplits);
        double r = 1.0 - l;

        // current coordinate of split surface
        currentSplitCoord = new PosVector(currentSplitCoord.X, currentSplitCoord.Y, bbox.BoxMin.Z + l * hz);

        var (vl, vr) = SplitBoundingBox(bbox, KdNodePlane.XY, currentSplitCoord);

        var currentSah = (sSplit + l * sNonSplit) * Convert.ToDouble(NumShapesInBoundingBox(shapes, vl)) +
                         (sSplit + r * sNonSplit) * Convert.ToDouble(NumShapesInBoundingBox(shapes, vr)) +
                         splitCost;

        if (currentSah < bestSah)
        {
          bestSah = currentSah;
          finalPlane = KdNodePlane.XY;
          finalPos = currentSplitCoord;
        }
      }

      // trying to minimize SAH by splitting across XZ plane
      sSplit = sxz;
      sNonSplit = sxy + syz;
      for (int i = 1; i < maxSplits; i++)
      {
        double l = Convert.ToDouble(i) / Convert.ToDouble(maxSplits);
        double r = 1.0 - l;

        // current coordinate of split surface
        currentSplitCoord = new PosVector(currentSplitCoord.X, bbox.BoxMin.Y + l * hy, currentSplitCoord.Z);

        var (vl, vr) = SplitBoundingBox(bbox, KdNodePlane.XZ, currentSplitCoord);

        var currentSah = (sSplit + l * sNonSplit) * Convert.ToDouble(NumShapesInBoundingBox(shapes, vl)) +
                         (sSplit + r * sNonSplit) * Convert.ToDouble(NumShapesInBoundingBox(shapes, vr)) +
                         splitCost;

        if (currentSah < bestSah)
        {
          bestSah = currentSah;
          finalPlane = KdNodePlane.XZ;
          finalPos = currentSplitCoord;
        }
      }

      // trying to minimize SAH by splitting across YZ plane
      sSplit = syz;
      sNonSplit = sxy + sxz;
      for (int i = 1; i < maxSplits; i++)
      {
        double l = Convert.ToDouble(i) / Convert.ToDouble(maxSplits);
        double r = 1.0 - l;

        // current coordinate of split surface
        currentSplitCoord = new PosVector(bbox.BoxMin.X + l * hy, currentSplitCoord.Y, currentSplitCoord.Z);

        var (vl, vr) = SplitBoundingBox(bbox, KdNodePlane.YZ, currentSplitCoord);

        var currentSah = (sSplit + l * sNonSplit) * Convert.ToDouble(NumShapesInBoundingBox(shapes, vl)) +
                         (sSplit + r * sNonSplit) * Convert.ToDouble(NumShapesInBoundingBox(shapes, vr)) +
                         splitCost;

        if (currentSah < bestSah)
        {
          bestSah = currentSah;
          finalPlane = KdNodePlane.YZ;
          finalPos = currentSplitCoord;
        }
      }

      return (finalPlane, finalPos);
    }

    private static int NumShapesInBoundingBox(List<IShape> shapes, BoundingBox bbox)
    {
      int count = 0;

      foreach (var shape in shapes)
      {
        if (IsShapeInBoundingBox(shape, bbox))
        {
          count++;
        }
      }

      return count;
    }
  }
}