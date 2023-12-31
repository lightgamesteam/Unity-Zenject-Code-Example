﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Module.Eclipses
{
    public class Point: IEqualityComparer<Point> {

								public float x, y, z;

								Vector3 _projectedVector3;
								bool _projectedVector3Computed;

								public Vector3 projectedVector3 {
												get {
																if (_projectedVector3Computed) {
																				return _projectedVector3;
																} else {
																				_projectedVector3 = new Vector3 (x, y, z).normalized * 0.5f;
																				_projectedVector3Computed = true;
																				return _projectedVector3;
																}
												}
								}

								public Triangle[] triangles; // = new Triangle[6];
								public int triangleCount;
								public Cell tile;

								int hashCode;

								public Point (float x, float y, float z) {
												this.x = x;
												this.y = y;
												this.z = z;
								}

								public List<Point>Subdivide (Point point, int count, GetCachedPointDelegate checkPoint) {
												List<Point> segments = new List<Point> (count + 1);
												segments.Add (this);
			
												for (int i = 1; i < count; i++) {
																Point np = new Point (this.x + (point.x - this.x) * i / count,
																				this.y + (point.y - this.y) * i / count,
																				this.z + (point.z - this.z) * i / count);
																np = checkPoint (np);
																segments.Add (np);
												}
			
												segments.Add (point);
			
												return segments;
			
								}


								public void RegisterTriangle (Triangle triangle) {
												if (triangles == null)
																triangles = new Triangle[6];
												triangles[triangleCount++] = triangle;
								}

								public static int flag = 0;

								public int GetOrderedTriangles (Triangle[] tempTriangles) {
												if (triangleCount == 0) {
																return 0;
												}
												tempTriangles [0] = triangles [0];
												int count = 1;
												flag++;
												for (int i = 0; i < triangleCount - 1; i++) {
																for (int j = 1; j < triangleCount; j++) {
																				if (triangles [j].getOrderedFlag != flag && tempTriangles [i] != null && triangles [j].isAdjacentTo (tempTriangles [i])) {
																								tempTriangles [count++] = triangles [j];
																								triangles [j].getOrderedFlag = flag;
																								break;
																				}
																}
												}

												return count;
								}

		
								public override string ToString () {
												return (int)(this.x * 100f) / 100f + "," + (int)(this.y * 100f) / 100f + "," + (int)(this.z * 100f) / 100f;
			
								}

								public override bool Equals (object obj) {
												if (obj is Point) {
																Point other = (Point)obj;
																return x == other.x && y == other.y && z == other.z;
												}
												return false;
								}

								public bool Equals (Point p2) {
												return x == p2.x && y == p2.y && z == p2.z;
								}

								public bool Equals (Point p1, Point p2) {
												return  p1.x == p2.x && p1.y == p2.y && p1.z == p2.z;
								}

								public override int GetHashCode () {
												if (hashCode == 0) {
																hashCode = x.GetHashCode () ^ y.GetHashCode () << 2 ^ z.GetHashCode () >> 2;
												}
												return hashCode;
								}

								public int GetHashCode (Point p) {
												if (hashCode == 0) {
																hashCode = p.x.GetHashCode () ^ p.y.GetHashCode () << 2 ^ p.z.GetHashCode () >> 2;
												}
												return hashCode;
								}

								public static explicit operator Vector3 (Point point) {
												return new Vector3 (point.x, point.y, point.z);
								}

								public static Point operator * (Point point, float v) {
												return new Point (point.x * v, point.y * v, point.z * v);
								}

				}

}
