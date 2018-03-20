#include "SVGCurveLib.h"
#include <Arduino.h>
#include <algorithm>
#include <math.h>
#include <cmath>
#include <array>
#include <vector>

const float PI_OVER_180 = (PI / 180);

SVGCurveLib::PointGeneric<> SVGCurveLib::PointOnLine(SVGCurveLib::PointGeneric<> p0, SVGCurveLib::PointGeneric<> p1, float t) {
  auto calculateLinearLineParameter = [](float x0, float x1, float t) {
    return x0 + (x1 - x0) * t;
  };

  return SVGCurveLib::Point(
           calculateLinearLineParameter(p0.x, p1.x, t),
           calculateLinearLineParameter(p0.y, p1.y, t)
         );
}

SVGCurveLib::PointGeneric<> SVGCurveLib::PointOnQuadraticBezierCurve(SVGCurveLib::PointGeneric<> p0, SVGCurveLib::PointGeneric<> p1, SVGCurveLib::PointGeneric<> p2, float t) {

  auto calculateQuadraticBezierParameter = [](float x0, float x1, float x2, float t) {
    return powf(1 - t, 2) * x0 + 2 * t * (1 - t) * x1 + powf(t, 2) * x2;
  };

  return SVGCurveLib::Point(
           calculateQuadraticBezierParameter(p0.x, p1.x, p2.x, t),
           calculateQuadraticBezierParameter(p0.y, p1.y, p2.y, t)
         );
}

SVGCurveLib::PointGeneric<> SVGCurveLib::PointOnCubicBezierCurve(SVGCurveLib::PointGeneric<> p0, SVGCurveLib::PointGeneric<> p1, SVGCurveLib::PointGeneric<> p2, SVGCurveLib::PointGeneric<> p3, float t) {
  auto calculateCubicBezierParameter = [](float x0, float x1, float x2, float x3, float t) {
    return powf(1 - t, 3) * x0 + 3 * t * powf(1 - t, 2) * x1 + 3 * (1 - t) * powf(t, 2) * x2 + powf(t, 3) * x3;
  };

  return SVGCurveLib::Point(
           calculateCubicBezierParameter(p0.x, p1.x, p2.x, p3.x, t),
           calculateCubicBezierParameter(p0.y, p1.y, p2.y, p3.y, t)
         );
}

SVGCurveLib::PointWithEllipticalArcInfo SVGCurveLib::PointOnEllipticalArc(SVGCurveLib::PointGeneric<> p0, float rx, float ry, float xAxisRotation, bool largeArcFlag, bool sweepFlag, SVGCurveLib::PointGeneric<> p1, float t) {

  rx = fabs(rx);
  ry = fabs(ry);
  xAxisRotation = fmod(xAxisRotation, 360.0f);
  float xAxisRotationRadians = SVGCurveLib::ToRadians(xAxisRotation);
  if (p0 == p1) {
    return p0;
  }

  if (rx == 0 || ry == 0) {
    return SVGCurveLib::PointOnLine(p0, p1, t);
  }

  float dx = (p0.x - p1.x) / 2;
  float dy = (p0.y - p1.y) / 2;
  auto transformedPoint = SVGCurveLib::Point(
                            cosf(xAxisRotationRadians) * dx + sinf(xAxisRotationRadians) * dy,
                            -sinf(xAxisRotationRadians) * dx + cosf(xAxisRotationRadians) * dy
                          );

  float radiiCheck = powf(transformedPoint.x, 2) / powf(rx, 2) + powf(transformedPoint.y, 2) / powf(ry, 2);
  if (radiiCheck > 1) {
    rx = sqrtf(radiiCheck) * rx;
    ry = sqrtf(radiiCheck) * ry;
  }

  float cSquareNumerator = powf(rx, 2) * powf(ry, 2) - powf(rx, 2) * powf(transformedPoint.y, 2) - powf(ry, 2) * powf(transformedPoint.x, 2);
  float cSquareRootDenom = powf(rx, 2) * powf(transformedPoint.y, 2) + powf(ry, 2) * powf(transformedPoint.x, 2);
  float cRadicand = cSquareNumerator / cSquareRootDenom;

  cRadicand = cRadicand < 0 ? 0 : cRadicand;
  float cCoef = (largeArcFlag != sweepFlag ? 1 : -1) * sqrtf(cRadicand);
  auto transformedCenter = SVGCurveLib::Point(
                             cCoef * ((rx * transformedPoint.y) / ry),
                             cCoef * (-(ry * transformedPoint.x) / rx)
                           );

  auto center = SVGCurveLib::Point(
                  cosf(xAxisRotationRadians) * transformedCenter.x - sinf(xAxisRotationRadians) * transformedCenter.y + ((p0.x + p1.x) / 2),
                  sinf(xAxisRotationRadians) * transformedCenter.x + cosf(xAxisRotationRadians) * transformedCenter.y + ((p0.y + p1.y) / 2)
                );

  auto startVector = SVGCurveLib::Point(
                       (transformedPoint.x - transformedCenter.x) / rx,
                       (transformedPoint.y - transformedCenter.y) / ry
                     );
  float startAngle = SVGCurveLib::AngleBetween(SVGCurveLib::Point(1.0f, 0.0f), startVector);

  auto endVector = SVGCurveLib::Point(
                     (-transformedPoint.x - transformedCenter.x) / rx,
                     (-transformedPoint.y - transformedCenter.y) / ry
                   );
  float sweepAngle = SVGCurveLib::AngleBetween(startVector, endVector);

  if (!sweepFlag && sweepAngle > 0) {
    sweepAngle -= TWO_PI;
  }
  else if (sweepFlag && sweepAngle < 0) {
    sweepAngle += TWO_PI;
  }

  sweepAngle = fmod(sweepAngle, TWO_PI);

  float angle = startAngle + (sweepAngle * t);
  float ellipseComponentX = rx * cosf(angle);
  float ellipseComponentY = ry * sinf(angle);

  auto point = SVGCurveLib::PointWithEllipticalArcInfo(
                 cosf(xAxisRotationRadians) * ellipseComponentX - sinf(xAxisRotationRadians) * ellipseComponentY + center.x,
                 sinf(xAxisRotationRadians) * ellipseComponentX + cosf(xAxisRotationRadians) * ellipseComponentY + center.y
               );

  point.ellipticalArcStartAngle = startAngle;
  point.ellipticalArcEndAngle = startAngle + sweepAngle;
  point.ellipticalArcAngle = angle;

  point.ellipticalArcCenter = SVGCurveLib::Point(float(center.x), float(center.y));
  point.resultantRx = rx;
  point.resultantRy = ry;

  return point;
}

template <typename T>
T SVGCurveLib::ToRadians(T angle) {
  return angle * PI_OVER_180;
};

template <typename Tx, typename Ty>
float SVGCurveLib::AngleBetween(Tx v0, Ty v1) {

  float p = v0.x * v1.x + v0.y * v1.y;
  float n = sqrtf((powf(v0.x, 2) + powf(v0.y, 2)) * (powf(v1.x, 2) + powf(v1.y, 2)));
  float sign = v0.x * v1.y - v0.y * v1.x < 0 ? -1.0f : 1.0f;
  float angle = sign * acosf(p / n);
  return angle;
};


