#ifndef SVGCurveLib_h
#define SVGCurveLib_h

#include <Arduino.h>
#include <algorithm>
#include <math.h>
#include <cmath>
#include <array>
#include <vector>

template<class T>
using decay_t = typename std::decay<T>::type;

class SVGCurveLib
{
  public:

    template <typename Tx = float, typename Ty = Tx>
    struct PointGeneric {

        struct x_getter {
          public:
            x_getter(PointGeneric *t)
              : thisPointer{t} {};

            operator Tx() const {
              return this->thisPointer->_x;
            };

          private:
            PointGeneric<Tx, Ty>* thisPointer;
        };
        x_getter x{this};

        struct y_getter {
          public:
            y_getter(PointGeneric *t)
              : thisPointer{t} {};

            operator Ty() const {
              return this->thisPointer->_y;
            };

          private:
            PointGeneric<Tx, Ty>* thisPointer;
        };
        y_getter y{this};



        PointGeneric()
          : PointGeneric { 0, 0 } {};

        PointGeneric(Tx x, Ty y)
          : x(this), y(this), _x{x}, _y{y} {};

        PointGeneric(const PointGeneric<Tx, Ty>& other)
          : x(this), y(this), _x(std::move(other._x)), _y(std::move(other._y)) {};


        bool equalsWithinTolerance(PointGeneric<Tx, Ty> other) {
          return equalsWithinTolerance(other, 0.0001f);
        };

        bool equalsWithinTolerance(PointGeneric<Tx, Ty> other, float tolerance) {
          return fabs(this->x - other.x) <= tolerance && fabs(this->y - other.y) <= tolerance;
        };

        bool operator == (const PointGeneric<Tx, Ty> &v) {
          return (this->x == v.x) && (this->y == v.y);
        }
        bool operator != (const PointGeneric<Tx, Ty> &v) {
          return !this->operator==(v);
        }


      private:
        Tx _x;
        Ty _y;
    };

    template <typename Tx = float, typename Ty = Tx>
    static PointGeneric<decay_t<Tx>, decay_t<Ty>> Point(Tx && x, Ty && y) {
      return { std::forward<Tx>(x), std::forward<Ty>(y) };
    };

    struct PointWithEllipticalArcInfo {
      SVGCurveLib::PointGeneric<> point;

      float ellipticalArcStartAngle = 0;
      float ellipticalArcEndAngle = 0;
      float ellipticalArcAngle = 0;
      SVGCurveLib::PointGeneric<> ellipticalArcCenter;
      float resultantRx = 0;
      float resultantRy = 0;

      PointWithEllipticalArcInfo() {};
      PointWithEllipticalArcInfo(SVGCurveLib::PointGeneric<> p) : point{p} {};
      PointWithEllipticalArcInfo(float x, float y) {
        this->point = SVGCurveLib::Point(x, y);
      };

      operator SVGCurveLib::PointGeneric<>() {
        return this->point;
      };
    };

    static SVGCurveLib::PointGeneric<> PointOnLine(SVGCurveLib::PointGeneric<> p0, SVGCurveLib::PointGeneric<> p1, float t);
    static SVGCurveLib::PointGeneric<> PointOnQuadraticBezierCurve(SVGCurveLib::PointGeneric<> p0, SVGCurveLib::PointGeneric<> p1, SVGCurveLib::PointGeneric<> p2, float t);
    static SVGCurveLib::PointGeneric<> PointOnCubicBezierCurve(SVGCurveLib::PointGeneric<> p0, SVGCurveLib::PointGeneric<> p1, SVGCurveLib::PointGeneric<> p2, SVGCurveLib::PointGeneric<> p3, float t);
    static SVGCurveLib::PointWithEllipticalArcInfo PointOnEllipticalArc(SVGCurveLib::PointGeneric<> p0, float rx, float ry, float xAxisRotation, bool largeArcFlag, bool sweepFlag, SVGCurveLib::PointGeneric<> p1, float t);

    static SVGCurveLib::PointGeneric<> CalculateQuadraticBezierTOfCriticalPoint(SVGCurveLib::PointGeneric<> p0, SVGCurveLib::PointGeneric<> p1, SVGCurveLib::PointGeneric<> p2);
    static std::array<SVGCurveLib::PointGeneric<>, 3> CalculateCubicBezierTOfCriticalPoint(SVGCurveLib::PointGeneric<> p0, SVGCurveLib::PointGeneric<> p1, SVGCurveLib::PointGeneric<> p2, SVGCurveLib::PointGeneric<> p3);


  private:
    template <typename T = float>
    static T ToRadians(T angle);

    template <typename Tx = SVGCurveLib::PointGeneric<>, typename Ty = Tx>
    static float AngleBetween(Tx v0, Ty v1);
};

#endif
