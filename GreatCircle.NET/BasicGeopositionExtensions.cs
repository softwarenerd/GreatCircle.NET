//
//  The MIT License (MIT)
//
//  Copyright (c) 2016 Softwarenerd.
//
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
//
//  The above copyright notice and this permission notice shall be included in
//  all copies or substantial portions of the Software.
//
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//  THE SOFTWARE.
//
//  BasicGeopositionExtensions.cs
//  GreatCircle.NET
//
//  Created by Brian Lambert on 8/5/16.
//  Copyright © 2016 Softwarenerd.
//

/*!
 *  @header
 *  This work was adapted from: https://github.com/chrisveness/geodesy
 *
 *  @abstract
 *  Geodesy functions for working with points and paths (distances, bearings, destinations, etc)
 *  on a spherical-model earth.
 */

using System;
using System.Runtime.CompilerServices;
using Windows.Devices.Geolocation;
using static System.Math;

namespace GreatCircle.NET
{
    /// <summary>
    /// Extensions to the <see cref="Windows.Devices.Geolocation.Geopoint"/> class.
    /// </summary>
    public static class BasicGeopositionExtensions
    {
        /// <summary>
        /// The radius of the earth in meters.
        /// </summary>
        private const double EARTH_RADIUS_IN_METERS = 6371000.0;

        /// <summary>
        /// Converts a value from degrees to radians.
        /// </summary>
        /// <param name="degrees">The degrees value.</param>
        /// <returns>The radians value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        // Converts degrees to radians.
        private static double ConvertDegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180.0;
        }

        /// <summary>
        /// Converts a value from radians to degrees.
        /// </summary>
        /// <param name="radians">The radians value.</param>
        /// <returns>The degrees value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        // Converts degrees to radians.
        private static double ConvertRadiansToDegrees(double radians)
        {
            return radians * 180.0 / Math.PI;
        }

        /// <summary>
        /// Compares two doubles to the specified number of decimal places. 
        /// </summary>
        private static bool CompareToDecimalPlaces(double value1, double value2, int decimalPlaces)
        {
            double multiplier = Pow(10, decimalPlaces);
            return Round(value1 * multiplier) == Round(value2 * multiplier);
        }

        /// <summary>
        /// Calculates a location representing the point of intersection of two paths, each specified by a location and bearing.
        /// </summary>
        /// <param name="position1">The first position.</param>
        /// <param name="bearing1">The first bearing.</param>
        /// <param name="position2">The second position.</param>
        /// <param name="bearing2">The second bearing.</param>
        /// <returns>A location representing the point of intersection of two paths, each specified by a location and bearing.</returns>
        public static BasicGeoposition? IntersectionOf(BasicGeoposition position1, double bearing1, BasicGeoposition position2, double bearing2)
        {
            // see http://williams.best.vwh.net/avform.htm#Intersection
            double φ1 = ConvertDegreesToRadians(position1.Latitude);
            double λ1 = ConvertDegreesToRadians(position1.Longitude);
            double φ2 = ConvertDegreesToRadians(position2.Latitude);
            double λ2 = ConvertDegreesToRadians(position2.Longitude);
            double θ13 = ConvertDegreesToRadians(bearing1);
            double θ23 = ConvertDegreesToRadians(bearing2);
            double Δφ = φ2 - φ1;
            double Δλ = λ2 - λ1;

            double δ12 = 2.0 * Asin(Sqrt(Sin(Δφ / 2.0) * Sin(Δφ / 2.0) + Cos(φ1) * Cos(φ2) * Sin(Δλ / 2.0) * Sin(Δλ / 2.0)));
            if (δ12 == 0.0)
            {
                return null;
            }

            // Initial/final bearings between points
            double θ1 = Acos((Sin(φ2) - Sin(φ1) * Cos(δ12)) / (Sin(δ12) * Cos(φ1)));
            if (double.IsNaN(θ1))
            {
                // Protect against rounding.
                θ1 = 0.0;
            }

            double θ2 = Acos((Sin(φ1) - Sin(φ2) * Cos(δ12)) / (Sin(δ12) * Cos(φ2)));

            double θ12 = Sin(λ2 - λ1) > 0.0 ? θ1 : 2.0 * PI - θ1;
            double θ21 = Sin(λ2 - λ1) > 0.0 ? 2.0 * PI - θ2 : θ2;

            double α1 = (θ13 - θ12 + PI) % ((2.0 * PI) - PI); // angle 2-1-3
            double α2 = (θ21 - θ23 + PI) % ((2.0 * PI) - PI); // angle 1-2-3

            // Infinite intersections.
            if (Sin(α1) == 0.0 && Sin(α2) == 0.0)
            {
                return null;
            }

            // Ambiguous intersection.
            if (Sin(α1) * Sin(α2) < 0.0)
            {
                return null;
            }

            //α1 = abs(α1);
            //α2 = abs(α2);
            // ... Ed Williams takes abs of α1/α2, but seems to break calculation?

            double α3 = Acos(-Cos(α1) * Cos(α2) + Sin(α1) * Sin(α2) * Cos(δ12));
            double δ13 = Atan2(Sin(δ12) * Sin(α1) * Sin(α2), Cos(α2) + Cos(α1) * Cos(α3));
            double φ3 = Asin(Sin(φ1) * Cos(δ13) + Cos(φ1) * Sin(δ13) * Cos(θ13));
            double Δλ13 = Atan2(Sin(θ13) * Sin(δ13) * Cos(φ1), Cos(δ13) - Sin(φ1) * Sin(φ3));
            double λ3 = λ1 + Δλ13;

            return new BasicGeoposition { Altitude = 0.0, Latitude = ConvertRadiansToDegrees(φ3), Longitude = ((ConvertRadiansToDegrees(λ3) + 540.0) % 360.0) - 180.0 };
        }

        /// <summary>
        /// Compares this position to the other position for equality.
        /// </summary>
        /// <param name="thisPosition">This position.</param>
        /// <param name="position">The other position.</param>
        /// <param name="includeAltitude">A value which indicates whether to include altitude in the comparison.</param>
        /// <returns>true, if the positions are the same; otherwise, false.</returns>
        public static bool EqualToPosition(this BasicGeoposition thisPosition, BasicGeoposition position, bool includeAltitude = false)
        {
            // If the positions are the same object, return true.
            if (Object.ReferenceEquals(thisPosition, position))
            {
                return true;
            }

            // Compare the latitude, longitude and, optionally, altitude.
            if (thisPosition.Latitude == position.Latitude &&
                thisPosition.Longitude == position.Longitude &&
                (includeAltitude ? thisPosition.Altitude == position.Altitude : true))
            {
                return true;
            }

            // The positions are not equal.
            return false;
        }


        /// <summary>
        /// Calculates the distance (in meters) between this position and the other position.
        /// </summary>
        /// <param name="thisPosition">This position.</param>
        /// <param name="position">The other position.</param>
        /// <returns>The distance (in meters) between this position and the other position.</returns>
        public static double DistanceToOtherPosition(this BasicGeoposition thisPosition, BasicGeoposition position)
        {
            // If the the two positions are the same, return 0.0; otherwise, calculate the distance between them.
            if (thisPosition.EqualToPosition(position))
            {
                return 0.0;
            }
            else
            {
                double φ1 = ConvertDegreesToRadians(thisPosition.Latitude);
                double λ1 = ConvertDegreesToRadians(thisPosition.Longitude);
                double φ2 = ConvertDegreesToRadians(position.Latitude);
                double λ2 = ConvertDegreesToRadians(position.Longitude);
                double Δφ = φ2 - φ1;
                double Δλ = λ2 - λ1;
                double a = Sin(Δφ / 2.0) * Sin(Δφ / 2.0) + Cos(φ1) * Cos(φ2) * Sin(Δλ / 2.0) * Sin(Δλ / 2.0);
                double c = 2.0 * Atan2(Sqrt(a), Sqrt(1.0 - a));
                double d = EARTH_RADIUS_IN_METERS * c;
                return d;
            }
        }

        /// <summary>
        /// Calculates the initial bearing (in degrees) between this position and the other position.
        /// </summary>
        /// <param name="thisPosition">This position.</param>
        /// <param name="position">The other position.</param>
        /// <returns>The initial bearing (in degrees) between this position and the other position.</returns>
        public static double InitialBearingToPosition(this BasicGeoposition thisPosition, BasicGeoposition position)
        {
            // If the position are the same, return 0.0; otherwise, calculate the initial bearing.
            if (thisPosition.EqualToPosition(position))
            {
                return 0.0;
            }
            else
            {
                double φ1 = ConvertDegreesToRadians(thisPosition.Latitude);
                double φ2 = ConvertDegreesToRadians(position.Latitude);
                double Δλ = ConvertDegreesToRadians(position.Longitude - thisPosition.Longitude);

                // see http://mathforum.org/library/drmath/view/55417.html
                double y = Sin(Δλ) * Cos(φ2);
                double x = Cos(φ1) * Sin(φ2) - Sin(φ1) * Cos(φ2) * Cos(Δλ);
                double θ = Atan2(y, x);
                return (ConvertRadiansToDegrees(θ) + 360.0) % 360.0;
            }
        }

        /// <summary>
        /// Calculates the final bearing (in degrees) between this position and the other position.
        /// </summary>
        /// <remarks>
        /// The final bearing will differ from the initial bearing by varying degrees according to distance and latitude.
        /// </remarks>
        /// <param name="thisPosition">This position.</param>
        /// <param name="position">The other position.</param>
        /// <returns>The final bearing (in degrees) between this position and the other position.</returns>
        public static double FinalBearingToPosition(this BasicGeoposition thisPosition, BasicGeoposition position)
        {
            // If the position are the same, return 0.0; otherwise, calculate the initial bearing.
            if (thisPosition.EqualToPosition(position))
            {
                return 0.0;
            }
            else
            {
                return (position.InitialBearingToPosition(thisPosition) + 180.0) % 360.0;
            }
        }


        /// <summary>
        /// Calculates a position representing the midpoint between this position and the other position.
        /// </summary>
        /// <param name="thisPosition">This position.</param>
        /// <param name="position">The other position.</param>
        /// <returns>A position representing the midpoint between this position and the other position.</returns>
        public static BasicGeoposition MidpointToPosition(this BasicGeoposition thisPosition, BasicGeoposition position)
        {
            // If the locations are the same, return self; otherwise, return a location representing the midpoint.
            if (thisPosition.EqualToPosition(position))
            {
                return thisPosition;
            }
            else
            {
                // φm = atan2( sinφ1 + sinφ2, √( (cosφ1 + cosφ2⋅cosΔλ) ⋅ (cosφ1 + cosφ2⋅cosΔλ) ) + cos²φ2⋅sin²Δλ )
                // λm = λ1 + atan2(cosφ2⋅sinΔλ, cosφ1 + cosφ2⋅cosΔλ)
                // see http://mathforum.org/library/drmath/view/51822.html for derivation

                double φ1 = ConvertDegreesToRadians(thisPosition.Latitude);
                double λ1 = ConvertDegreesToRadians(thisPosition.Longitude);

                double φ2 = ConvertDegreesToRadians(position.Latitude);
                double Δλ = ConvertDegreesToRadians(position.Longitude - thisPosition.Longitude);

                double Bx = Cos(φ2) * Cos(Δλ);
                double By = Cos(φ2) * Sin(Δλ);

                double x = Sqrt((Cos(φ1) + Bx) * (Cos(φ1) + Bx) + By * By);
                double y = Sin(φ1) + Sin(φ2);
                double φ3 = Atan2(y, x);

                double λ3 = λ1 + Atan2(By, Cos(φ1) + Bx);

                return new BasicGeoposition { Altitude = 0.0, Latitude = ConvertRadiansToDegrees(φ3), Longitude = ((ConvertRadiansToDegrees(λ3) + 540.0) % 360.0) - 180.0 };
            }
        }

        /// <summary>
        /// Calculates a position representing the point that lies at the specified bearing and distance from this position.
        /// </summary>
        /// <param name="thisPosition">This position.</param>
        /// <param name="bearing">The bearing.</param>
        /// <param name="distance">The distance.</param>
        /// <returns>A position representing the point that lies at the specified bearing and distance from this position.</returns>
        public static BasicGeoposition LocationWithBearingAndDistance(this BasicGeoposition thisPosition, double bearing, double distance)
        {
            if (distance == 0.0)
            {
                return thisPosition;
            }
            else
            {
                // φ2 = asin( sinφ1⋅cosδ + cosφ1⋅sinδ⋅cosθ )
                // λ2 = λ1 + atan2( sinθ⋅sinδ⋅cosφ1, cosδ − sinφ1⋅sinφ2 )
                // see http://williams.best.vwh.net/avform.htm#LL

                double δ = distance / EARTH_RADIUS_IN_METERS; // angular distance in radians
                double θ = ConvertDegreesToRadians(bearing);

                double φ1 = ConvertDegreesToRadians(thisPosition.Latitude);
                double λ1 = ConvertDegreesToRadians(thisPosition.Longitude);

                double φ2 = Asin(Sin(φ1) * Cos(δ) + Cos(φ1) * Sin(δ) * Cos(θ));
                double x = Cos(δ) - Sin(φ1) * Sin(φ2);
                double y = Sin(θ) * Sin(δ) * Cos(φ1);
                double λ2 = λ1 + Atan2(y, x);

                return new BasicGeoposition { Altitude = 0.0, Latitude = ConvertRadiansToDegrees(φ2), Longitude = ((ConvertRadiansToDegrees(λ2) + 540.0) % 360.0) - 180.0 };
            }
        }

        /// <summary>
        /// Calculates the cross track distance of this position relative to the specified start position and end position.
        /// </summary>
        /// <param name="thisPosition">This position.</param>
        /// <param name="startPosition">The start position.</param>
        /// <param name="endPosititon">The end position.</param>
        /// <returns>The cross track distance of this position relative to the specified start position and end position.</returns>
        public static double CrossTrackDistance(this BasicGeoposition thisPosition, BasicGeoposition startPosition, BasicGeoposition endPosititon)
        {
            if (thisPosition.EqualToPosition(startPosition) || thisPosition.EqualToPosition(endPosititon))
            {
                return 0.0;
            }
            else
            {
                double δ13 = startPosition.DistanceToOtherPosition(thisPosition) / EARTH_RADIUS_IN_METERS;
                double θ13 = ConvertDegreesToRadians(startPosition.InitialBearingToPosition(thisPosition));
                double θ12 = ConvertDegreesToRadians(startPosition.InitialBearingToPosition(endPosititon));

                double dxt = Asin(Sin(δ13) * Sin(θ13 - θ12)) * EARTH_RADIUS_IN_METERS;

                return dxt;
            }
        }

        /// <summary>
        /// Calculates a position representing the cross track point of this location relative to the specified start position and end position.
        /// </summary>
        /// <param name="thisPosition">This position.</param>
        /// <param name="startPosition">The start position.</param>
        /// <param name="endPosititon">The end position.</param>
        /// <returns>A position representing the cross track point of this location relative to the specified start position and end position.</returns>
        public static BasicGeoposition CrossTrackPosition(this BasicGeoposition thisPosition, BasicGeoposition startPosition, BasicGeoposition endPosititon)
        {
            if (thisPosition.EqualToPosition(startPosition) || thisPosition.EqualToPosition(endPosititon))
            {
                return thisPosition;
            }
            else
            {
                // Calculate the cross-track distance.
                double crossTrackDistance = thisPosition.CrossTrackDistance(startPosition, endPosititon);

                // If the cross track distance is effectively 0.0, then return self as it's a location on the track.
                if (CompareToDecimalPlaces(Abs(crossTrackDistance), 0.0, 3))
                {
                    return thisPosition;
                }
                else
                {
                    return thisPosition.LocationWithBearingAndDistance((startPosition.InitialBearingToPosition(endPosititon) + (crossTrackDistance < 0.0 ? 90.0 : 270.0) + 360.0) % 360.0, Abs(crossTrackDistance));
                }
            }
        }
    }
}