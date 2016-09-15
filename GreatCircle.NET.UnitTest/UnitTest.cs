using System;
using Windows.Devices.Geolocation;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using static System.Math;

namespace GreatCircle.NET.UnitTest
{
    [TestClass]
    public class UnitTest
    {
        /// <summary>
        /// Returns a new instance of the Indian Pond position.
        /// </summary>
        private static BasicGeoposition PositionIndianPond => new BasicGeoposition { Altitude = 0.0, Latitude = 43.930912, Longitude = -72.053811 };

        /// <summary>
        /// Returns a new instance of the Eiffel Tower position.
        /// </summary>
        private static BasicGeoposition PositionEiffelTower => new BasicGeoposition { Altitude = 0.0, Latitude = 48.858158, Longitude = 2.294825 };

        /// <summary>
        /// Returns a new instance of the Versailles position.
        /// </summary>
        private static BasicGeoposition PositionVersailles => new BasicGeoposition { Altitude = 0.0, Latitude = 48.804766, Longitude = 2.120339 };

        /// <summary>
        /// Compares two doubles to the specified number of decimal places. 
        /// </summary>
        private static bool CompareToDecimalPlaces(double value1, double value2, int decimalPlaces)
        {
            var multiplier = Pow(10, decimalPlaces);
            return Round(value1 * multiplier) == Round(value2 * multiplier);
        }

        /// <summary>
        /// Distance between the Eiffel Tower and Versailles.
        /// </summary>
        const double DISTANCE_EEIFFEL_TOWER_TO_VERSAILLES = 14084.280704919684;

        /// <summary>
        /// The initial and final bearings between the Eiffel Tower and Versailles.
        /// </summary>
        const double INITIAL_BEARING_EIFFEL_TOWER_TO_VERSAILLES = 245.13460296861962;
        const double FINAL_BEARING_EIFFEL_TOWER_TO_VERSAILLES = 245.00325395138532;

        /// <summary>
        /// The initial and final bearings between Versailles and the Eiffel Tower.
        /// </summary>
        const double INITIAL_BEARING_VERSAILLES_TO_EIFFEL_TOWER = 65.003253951385318;
        const double FINAL_BEARING_VERSAILLES_TO_EIFFEL_TOWER = 65.134602968619618;

        /// <summary>
        /// Tests <see cref="InitialBearingToPosition"/> for two positions that are the same.
        /// </summary>
        [TestMethod]
        public void testInitialBearingSamePositions()
        {
            // Setup.
            var positionIndianPond = PositionIndianPond;

            // Test. This tests the detection of the same object being passed in as the source and other location.
            var bearing = positionIndianPond.InitialBearingToPosition(positionIndianPond);

            // Asset.
            Assert.AreEqual(bearing, 0.0);
        }

        /// <summary>
        /// Tests <see cref="InitialBearingToPosition"/> for two positions that are equal.
        /// </summary>
        [TestMethod]
        public void testInitialBearingEqualPositions()
        {
            // Test. This tests the detection of the same object being passed in as the source and other location.
            var bearing = PositionIndianPond.InitialBearingToPosition(PositionIndianPond);

            // Asset.
            Assert.AreEqual(bearing, 0.0);
        }

        /// <summary>
        /// Tests <see cref="FinalBearingToPosition"/> for two positions that are the same.
        /// </summary>
        /// 
        [TestMethod]
        public void testFinalBearingSamePositions()
        {
            // Setup.
            var positionIndianPond = PositionIndianPond;

            // Test. This tests the detection of the same object being passed in as the source and other location.
            var bearing = positionIndianPond.FinalBearingToPosition(positionIndianPond);

            // Asset.
            Assert.AreEqual(bearing, 0.0);
        }

        /// <summary>
        /// Tests <see cref="FinalBearingToPosition"/> for two positions that are equal.
        /// </summary>
        [TestMethod]
        public void testFinalBearingEqualPositions()
        {
            // Test. This tests the detection of the same object being passed in as the source and other location.
            var bearing = PositionIndianPond.FinalBearingToPosition(PositionIndianPond);

            // Asset.
            Assert.AreEqual(bearing, 0.0);
        }

        /// <summary>
        /// Tests the <see cref="DistanceToOtherPosition"/> method for the same location.
        /// </summary>
        [TestMethod]
        public void TestDistanceSameLocation()
        {
            // Setup.
            var positionIndianPond = PositionIndianPond;

            // Test.
            var distance = positionIndianPond.DistanceToOtherPosition(positionIndianPond);

            // Assert.
            Assert.AreEqual(0.0, distance);
        }

        /// <summary>
        /// Tests the <see cref="DistanceToOtherPosition"/> method for equal locations.
        /// </summary>
        [TestMethod]
        public void TestDistanceEqualLocation()
        {
            // Test.
            var distance = PositionIndianPond.DistanceToOtherPosition(PositionIndianPond);

            // Assert.
            Assert.AreEqual(0.0, distance);
        }

        /// <summary>
        /// Tests distance between Eiffel Tower and Versailles.
        /// </summary>
        [TestMethod]
        public void TestDistanceEiffelTowerToVersailles()
        {
            // Test.
            var distance = PositionEiffelTower.DistanceToOtherPosition(PositionVersailles);

            // Asset.
            Assert.AreEqual(distance, DISTANCE_EEIFFEL_TOWER_TO_VERSAILLES);
        }

        /// <summary>
        /// Tests distance between Versailles and Eiffel Tower.
        /// </summary>
        [TestMethod]
        public void testDistanceVersaillesToEiffelTower()
        {
            // Test.
            var distance = PositionVersailles.DistanceToOtherPosition(PositionEiffelTower);

            // Asset.
            Assert.AreEqual(distance, DISTANCE_EEIFFEL_TOWER_TO_VERSAILLES);
        }

        // Tests initial bearing between Eiffel Tower and Versailles.
        [TestMethod]
        public void testInitialBearingEiffelTowerToVersailles()
        {
            // Test.
            var bearing = PositionEiffelTower.InitialBearingToPosition(PositionVersailles);

            // Asset.
            Assert.AreEqual(bearing, INITIAL_BEARING_EIFFEL_TOWER_TO_VERSAILLES);
        }

        // Tests initial bearing between Versailles and Eiffel Tower.
        [TestMethod]
        public void testInitialBearingVersaillesToEiffelTower()
        {
            // Test.
            var bearing = PositionVersailles.InitialBearingToPosition(PositionEiffelTower);

            // Asset.
            Assert.AreEqual(bearing, INITIAL_BEARING_VERSAILLES_TO_EIFFEL_TOWER);
        }

        // Tests final bearing between Eiffel Tower and Versailles.
        [TestMethod]
        public void testFinalBearingEiffelTowerToVersailles()
        {
            // Test.
            var bearing = PositionEiffelTower.FinalBearingToPosition(PositionVersailles);

            // Asset.
            Assert.AreEqual(bearing, FINAL_BEARING_EIFFEL_TOWER_TO_VERSAILLES);
        }


        // Tests final bearing between Versailles and Eiffel Tower.
        [TestMethod]
        public void testFinalBearingVersaillesToEiffelTower()
        {
            // Test.
            var bearing = PositionVersailles.FinalBearingToPosition(PositionEiffelTower);

            // Asset.
            Assert.AreEqual(bearing, FINAL_BEARING_VERSAILLES_TO_EIFFEL_TOWER);
        }


        // Tests generating a location for Versailles based on bearing and distance.
        [TestMethod]
        public void testGenerateLocationVersailles()
        {
            // Setup.
            var positionVersailles = PositionVersailles;

            // Test.
            var positionVersaillesGenerated = PositionEiffelTower.LocationWithBearingAndDistance(INITIAL_BEARING_EIFFEL_TOWER_TO_VERSAILLES, DISTANCE_EEIFFEL_TOWER_TO_VERSAILLES);

            // Assert.
            Assert.IsTrue(CompareToDecimalPlaces(positionVersailles.Latitude, positionVersaillesGenerated.Latitude, 9));
            Assert.IsTrue(CompareToDecimalPlaces(positionVersailles.Longitude, positionVersaillesGenerated.Longitude, 9));
        }

        // Tests generating a location for Eiffel Tower based on bearing and distance.
        [TestMethod]
        public void testGenerateLocationEiffelTower()
        {
            // Setup.
            var positionEiffelTower = PositionEiffelTower;

            // Test.
            var positionEiffelTowerGenerated = PositionVersailles.LocationWithBearingAndDistance(INITIAL_BEARING_VERSAILLES_TO_EIFFEL_TOWER, DISTANCE_EEIFFEL_TOWER_TO_VERSAILLES);

            // Assert.
            Assert.IsTrue(CompareToDecimalPlaces(positionEiffelTower.Latitude, positionEiffelTowerGenerated.Latitude, 8));
            Assert.IsTrue(CompareToDecimalPlaces(positionEiffelTower.Longitude, positionEiffelTowerGenerated.Longitude, 8));
        }

        // Tests the midpoint between the Eiffel Tower and Versailles.
        [TestMethod]
        public void testMidpointEiffelTowerToVersailles()
        {
            // Setup.
            var positionEiffelTower = PositionEiffelTower;
            var positionVersailles = PositionVersailles;
            var halfDistanceEiffelTowerToVersailles = DISTANCE_EEIFFEL_TOWER_TO_VERSAILLES / 2.0;

            // Test.
            var midpointA = positionEiffelTower.MidpointToPosition(positionVersailles);

            // Assert.
            var midpointB = positionEiffelTower.LocationWithBearingAndDistance(INITIAL_BEARING_EIFFEL_TOWER_TO_VERSAILLES, halfDistanceEiffelTowerToVersailles);
            var distanceA = positionEiffelTower.DistanceToOtherPosition(midpointA);
            var distanceB = positionEiffelTower.DistanceToOtherPosition(midpointB);
            Assert.IsTrue(CompareToDecimalPlaces(distanceA, halfDistanceEiffelTowerToVersailles, 8));
            Assert.IsTrue(CompareToDecimalPlaces(distanceB, halfDistanceEiffelTowerToVersailles, 8));
            Assert.IsTrue(CompareToDecimalPlaces(midpointA.Latitude, midpointB.Latitude, 8));
            Assert.IsTrue(CompareToDecimalPlaces(midpointA.Longitude, midpointB.Longitude, 8));
        }

        // Tests the midpoint between Versailles and the Eiffel Tower.
        [TestMethod]
        public void testMidpointVersaillesToEiffelTower()
        {
            var positionVersailles = PositionVersailles;
            var positionEiffelTower = PositionEiffelTower;
            var halfDistanceEiffelTowerToVersailles = DISTANCE_EEIFFEL_TOWER_TO_VERSAILLES / 2.0;

            // Test.
            var midpointA = positionVersailles.MidpointToPosition(positionEiffelTower);

            // Assert.
            var midpointB = positionVersailles.LocationWithBearingAndDistance(INITIAL_BEARING_VERSAILLES_TO_EIFFEL_TOWER, halfDistanceEiffelTowerToVersailles);
            var distanceA = positionVersailles.DistanceToOtherPosition(midpointA);
            var distanceB = positionVersailles.DistanceToOtherPosition(midpointB);
            Assert.IsTrue(CompareToDecimalPlaces(distanceA, halfDistanceEiffelTowerToVersailles, 8));
            Assert.IsTrue(CompareToDecimalPlaces(distanceB, halfDistanceEiffelTowerToVersailles, 8));
            Assert.IsTrue(CompareToDecimalPlaces(midpointA.Latitude, midpointB.Latitude, 8));
            Assert.IsTrue(CompareToDecimalPlaces(midpointA.Longitude, midpointB.Longitude, 8));
    }

        // Test intersection.
        [TestMethod]
        public void testIntersection()
        {
            // Setup.
            var positionSaintGermain = new BasicGeoposition { Altitude = 0.0, Latitude = 48.897728, Longitude = 2.094977 };
            var positionOrly = new BasicGeoposition { Altitude = 0.0, Latitude = 48.747114, Longitude = 2.400526 };

            // Test.
            var position = BasicGeopositionExtensions.IntersectionOf(positionSaintGermain, positionSaintGermain.InitialBearingToPosition(positionOrly), PositionEiffelTower, INITIAL_BEARING_EIFFEL_TOWER_TO_VERSAILLES);

            // Assert.
            Assert.IsTrue(position.HasValue);
            Assert.IsTrue(CompareToDecimalPlaces(position.Value.Latitude, 48.83569094988361, 8));
            Assert.IsTrue(CompareToDecimalPlaces(position.Value.Longitude, 2.2212520313073583, 8));
        }

        // Cross-track distance test of a point 90° and 200 meters away.
        [TestMethod]
        public void testCrossTrackDistance90Degrees200Meters()
        {
            // Setup.
            var positionEiffelTower = PositionEiffelTower;
            var positionVersailles = PositionVersailles;
            var midpoint = positionEiffelTower.MidpointToPosition(PositionVersailles);
            var bearing = positionEiffelTower.InitialBearingToPosition(PositionVersailles);
            var testBearing = (bearing + 90.0) % 360.0;
            var testLocation = midpoint.LocationWithBearingAndDistance(testBearing, 200.0);

            // Test.
            var distance = testLocation.CrossTrackDistance(positionEiffelTower, positionVersailles);

            // Assert.
            Assert.IsTrue(CompareToDecimalPlaces(distance, 200.0, 3));
        }

        // Cross-track distance test of a point 270° and 200 meters away.
        [TestMethod]
        public void testCrossTrackDistance270Degrees200Meters()
        {
            // Setup.
            var positionEiffelTower = PositionEiffelTower;
            var positionVersailles = PositionVersailles;
            var midpoint = positionEiffelTower.MidpointToPosition(PositionVersailles);
            var bearing = positionEiffelTower.InitialBearingToPosition(PositionVersailles);
            var testBearing = (bearing + 270.0) % 360.0;
            var testLocation = midpoint.LocationWithBearingAndDistance(testBearing, 200.0);

            // Test.
            var distance = testLocation.CrossTrackDistance(positionEiffelTower, positionVersailles);

            // Assert.
            Assert.IsTrue(CompareToDecimalPlaces(distance, -200.0, 3));
        }

        // Cross-track distance that should be very close to 0.
        [TestMethod]
        public void testCrossTrackDistanceThatShouldBeVeryCloseToZero()
        {
            // Setup.
            var positionEiffelTower = PositionEiffelTower;
            var positionVersailles = PositionVersailles;
            var midpoint = positionEiffelTower.MidpointToPosition(positionVersailles);

            // Test.
            var distance = Abs(midpoint.CrossTrackDistance(positionEiffelTower, positionVersailles));

            // Assert.
            Assert.IsTrue(CompareToDecimalPlaces(distance, 0.0, 8));
        }

        // Cross-track point for a point on the line.
        [TestMethod]
        public void testCrossTrackPointThatShouldBeOnTheLine()
        {
            // Setup.
            var positionEiffelTower = PositionEiffelTower;
            var positionVersailles = PositionVersailles;
            var midpoint = positionEiffelTower.MidpointToPosition(positionVersailles);

            // Test.
            var crossTrackPosition = midpoint.CrossTrackPosition(positionEiffelTower, positionVersailles);

            // Assert.
            Assert.IsTrue(CompareToDecimalPlaces(midpoint.DistanceToOtherPosition(crossTrackPosition), 0.0, 8));
        }

        // Cross-track point for a point on the line.
        [TestMethod]
        public void testCrossTrackPointA()
        {
            // Setup.
            var positionEiffelTower = PositionEiffelTower;
            var positionVersailles = PositionVersailles;
            var midpoint = positionEiffelTower.MidpointToPosition(positionVersailles);
            var bearing = positionEiffelTower.InitialBearingToPosition(positionVersailles);
            var testBearing = (bearing + 90.0) % 360.0;
            var testLocation = midpoint.LocationWithBearingAndDistance(testBearing, 200.0);

            // Test.
            var crossTrackLocation = testLocation.CrossTrackPosition(positionEiffelTower, positionVersailles);

            // Assert.
            var distance = Abs(midpoint.DistanceToOtherPosition(crossTrackLocation));
            Assert.IsTrue(CompareToDecimalPlaces(distance, 0.0, 2));
        }

        // Cross-track point for a point on the line.
        [TestMethod]
        public void testCrossTrackPointB()
        {
            var positionEiffelTower = PositionEiffelTower;
            var positionVersailles = PositionVersailles;
            var midpoint = positionEiffelTower.MidpointToPosition(positionVersailles);
            var bearing = positionEiffelTower.InitialBearingToPosition(positionVersailles);
            var testBearing = (bearing + 270.0) % 360.0;
            var testLocation = midpoint.LocationWithBearingAndDistance(testBearing, 200.0);

            // Test.
            var crossTrackLocation = testLocation.CrossTrackPosition(positionEiffelTower, positionVersailles);

            // Assert.
            var distance = Abs(midpoint.DistanceToOtherPosition(crossTrackLocation));
            Assert.IsTrue(CompareToDecimalPlaces(distance, 0.0, 2));
        }
    }
}
