using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LuYao.Globalization
{
    [TestClass]
    public class UnitConverterTests
 {
        private UnitConverter _converter;

      [TestInitialize]
        public void Initialize()
        {
        _converter = new UnitConverter();
        }

        #region Length Conversion Tests

        [TestMethod]
        public void TryExchange_MetersToKilometers_ReturnsCorrectValue()
        {
         // Arrange & Act
         bool success = _converter.TryExchange("meters", "kilometers", 1000m, out decimal result);

            // Assert
       Assert.IsTrue(success);
 Assert.AreEqual(1m, result);
   }

        [TestMethod]
   public void TryExchange_InchesToCentimeters_ReturnsCorrectValue()
{
    // Arrange & Act
        bool success = _converter.TryExchange("inches", "centimeters", 10m, out decimal result);

            // Assert
    Assert.IsTrue(success);
         Assert.AreEqual(25.4m, result);
     }

        [TestMethod]
        public void TryExchange_FeetToMeters_ReturnsCorrectValue()
{
        // Arrange & Act
 bool success = _converter.TryExchange("feet", "meters", 10m, out decimal result);

            // Assert
            Assert.IsTrue(success);
    Assert.AreEqual(3.048m, result);
      }

        [TestMethod]
        public void TryExchange_MilesToKilometers_ReturnsCorrectValue()
        {
   // Arrange & Act
            bool success = _converter.TryExchange("miles", "kilometers", 1m, out decimal result);

          // Assert
            Assert.IsTrue(success);
  Assert.AreEqual(1.609344m, result);
        }

        [TestMethod]
        public void TryExchange_MillimetersToInches_ReturnsCorrectValue()
        {
   // Arrange & Act
       bool success = _converter.TryExchange("millimeters", "inches", 254m, out decimal result);

    // Assert
          Assert.IsTrue(success);
         Assert.AreEqual(10m, result, 0.0001m);
     }

    #endregion

        #region Mass Conversion Tests

  [TestMethod]
  public void TryExchange_KilogramsToGrams_ReturnsCorrectValue()
        {
 // Arrange & Act
     bool success = _converter.TryExchange("kilograms", "grams", 1m, out decimal result);

            // Assert
   Assert.IsTrue(success);
  Assert.AreEqual(1000m, result);
        }

     [TestMethod]
        public void TryExchange_PoundsToKilograms_ReturnsCorrectValue()
        {
            // Arrange & Act
 bool success = _converter.TryExchange("pounds", "kilograms", 10m, out decimal result);

            // Assert
      Assert.IsTrue(success);
   Assert.AreEqual(4.5359237m, result);
        }

      [TestMethod]
        public void TryExchange_OuncesToGrams_ReturnsCorrectValue()
  {
    // Arrange & Act
      bool success = _converter.TryExchange("ounces", "grams", 1m, out decimal result);

     // Assert
   Assert.IsTrue(success);
Assert.AreEqual(28.349523125m, result);
        }

   #endregion

    #region Temperature Conversion Tests

        [TestMethod]
        public void TryExchange_CelsiusToFahrenheit_ReturnsCorrectValue()
        {
            // Arrange & Act
bool success = _converter.TryExchange("degrees_celsius", "degrees_fahrenheit", 100m, out decimal result);

 // Assert
 Assert.IsTrue(success);
          Assert.AreEqual(212m, result);
        }

        [TestMethod]
        public void TryExchange_FahrenheitToCelsius_ReturnsCorrectValue()
        {
         // Arrange & Act
 bool success = _converter.TryExchange("degrees_fahrenheit", "degrees_celsius", 32m, out decimal result);

            // Assert
        Assert.IsTrue(success);
            Assert.AreEqual(0m, result);
        }

        [TestMethod]
     public void TryExchange_CelsiusToKelvin_ReturnsCorrectValue()
{
            // Arrange & Act
   bool success = _converter.TryExchange("degrees_celsius", "kelvin", 0m, out decimal result);

   // Assert
            Assert.IsTrue(success);
         Assert.AreEqual(273.15m, result);
        }

   [TestMethod]
     public void TryExchange_KelvinToCelsius_ReturnsCorrectValue()
        {
            // Arrange & Act
            bool success = _converter.TryExchange("kelvin", "degrees_celsius", 373.15m, out decimal result);

      // Assert
 Assert.IsTrue(success);
     Assert.AreEqual(100m, result);
        }

        [TestMethod]
      public void TryExchange_FahrenheitToKelvin_ReturnsCorrectValue()
        {
  // Arrange & Act
   bool success = _converter.TryExchange("degrees_fahrenheit", "kelvin", 32m, out decimal result);

            // Assert
 Assert.IsTrue(success);
        Assert.AreEqual(273.15m, result);
        }

        #endregion

        #region Volume Conversion Tests

        [TestMethod]
        public void TryExchange_LitersToMilliliters_ReturnsCorrectValue()
 {
   // Arrange & Act
   bool success = _converter.TryExchange("liters", "milliliters", 1m, out decimal result);

            // Assert
            Assert.IsTrue(success);
       Assert.AreEqual(1000m, result);
        }

        [TestMethod]
        public void TryExchange_GallonsToLiters_ReturnsCorrectValue()
    {
      // Arrange & Act
     bool success = _converter.TryExchange("gallons", "liters", 1m, out decimal result);

      // Assert
       Assert.IsTrue(success);
 Assert.AreEqual(3.785411784m, result);
        }

        [TestMethod]
        public void TryExchange_CubicMeterToLiters_ReturnsCorrectValue()
        {
            // Arrange & Act
            bool success = _converter.TryExchange("cubic_meters", "liters", 1m, out decimal result);

     // Assert
            Assert.IsTrue(success);
            Assert.AreEqual(1000m, result);
        }

        #endregion

        #region Speed Conversion Tests

  [TestMethod]
public void TryExchange_MetersPerSecondToKilometersPerHour_ReturnsCorrectValue()
     {
  // Arrange & Act
  bool success = _converter.TryExchange("meters_per_second", "kilometers_per_hour", 1m, out decimal result);

       // Assert
            Assert.IsTrue(success);
  Assert.AreEqual(3.6m, result, 0.01m);
        }

      [TestMethod]
        public void TryExchange_MilesPerHourToMetersPerSecond_ReturnsCorrectValue()
  {
            // Arrange & Act
        bool success = _converter.TryExchange("miles_per_hour", "meters_per_second", 1m, out decimal result);

    // Assert
        Assert.IsTrue(success);
        Assert.AreEqual(0.44704m, result);
   }

        #endregion

#region Area Conversion Tests

  [TestMethod]
        public void TryExchange_SquareMetersToSquareFeet_ReturnsCorrectValue()
        {
// Arrange & Act
            bool success = _converter.TryExchange("square_meters", "square_feet", 1m, out decimal result);

      // Assert
     Assert.IsTrue(success);
         Assert.AreEqual(10.7639104m, result, 0.0001m);
   }

        [TestMethod]
        public void TryExchange_HectaresToSquareMeters_ReturnsCorrectValue()
        {
       // Arrange & Act
            bool success = _converter.TryExchange("hectares", "square_meters", 1m, out decimal result);

            // Assert
            Assert.IsTrue(success);
      Assert.AreEqual(10000m, result);
        }

        [TestMethod]
        public void TryExchange_AcresToSquareMeters_ReturnsCorrectValue()
    {
  // Arrange & Act
        bool success = _converter.TryExchange("acres", "square_meters", 1m, out decimal result);

      // Assert
Assert.IsTrue(success);
 Assert.AreEqual(4046.8564224m, result);
        }

 #endregion

   #region Data Storage Conversion Tests

        [TestMethod]
public void TryExchange_BytesToKilobytes_ReturnsCorrectValue()
     {
     // Arrange & Act
            bool success = _converter.TryExchange("bytes", "kilobytes", 1024m, out decimal result);

      // Assert
       Assert.IsTrue(success);
     Assert.AreEqual(1m, result);
        }

        [TestMethod]
        public void TryExchange_MegabytesToBytes_ReturnsCorrectValue()
        {
        // Arrange & Act
            bool success = _converter.TryExchange("megabytes", "bytes", 1m, out decimal result);

            // Assert
     Assert.IsTrue(success);
       Assert.AreEqual(1048576m, result);
    }

        [TestMethod]
        public void TryExchange_GigabytesToMegabytes_ReturnsCorrectValue()
    {
      // Arrange & Act
            bool success = _converter.TryExchange("gigabytes", "megabytes", 1m, out decimal result);

     // Assert
            Assert.IsTrue(success);
            Assert.AreEqual(1024m, result);
        }

        #endregion

        #region Energy Conversion Tests

        [TestMethod]
        public void TryExchange_JoulesToKilojoules_ReturnsCorrectValue()
        {
   // Arrange & Act
            bool success = _converter.TryExchange("joules", "kilojoules", 1000m, out decimal result);

  // Assert
            Assert.IsTrue(success);
  Assert.AreEqual(1m, result);
        }

        [TestMethod]
        public void TryExchange_CaloriesToJoules_ReturnsCorrectValue()
        {
            // Arrange & Act
       bool success = _converter.TryExchange("calories", "joules", 1m, out decimal result);

            // Assert
         Assert.IsTrue(success);
            Assert.AreEqual(4.184m, result);
   }

        [TestMethod]
        public void TryExchange_KilocaloriesToCalories_ReturnsCorrectValue()
        {
 // Arrange & Act
   bool success = _converter.TryExchange("kilocalories", "calories", 1m, out decimal result);

         // Assert
            Assert.IsTrue(success);
     Assert.AreEqual(1000m, result);
        }

        #endregion

        #region Edge Cases and Error Handling Tests

      [TestMethod]
        public void TryExchange_SameUnit_ReturnsOriginalValue()
        {
            // Arrange & Act
          bool success = _converter.TryExchange("meters", "meters", 100m, out decimal result);

            // Assert
            Assert.IsTrue(success);
            Assert.AreEqual(100m, result);
        }

        [TestMethod]
        public void TryExchange_CaseInsensitive_ReturnsCorrectValue()
        {
         // Arrange & Act
      bool success = _converter.TryExchange("METERS", "KILOMETERS", 1000m, out decimal result);

   // Assert
   Assert.IsTrue(success);
    Assert.AreEqual(1m, result);
   }

        [TestMethod]
        public void TryExchange_IncompatibleUnits_ReturnsFalse()
        {
          // Arrange & Act
         bool success = _converter.TryExchange("meters", "kilograms", 10m, out decimal result);

 // Assert
     Assert.IsFalse(success);
 Assert.AreEqual(0m, result);
     }

        [TestMethod]
        public void TryExchange_UnknownFromUnit_ReturnsFalse()
      {
            // Arrange & Act
  bool success = _converter.TryExchange("unknown_unit", "meters", 10m, out decimal result);

         // Assert
       Assert.IsFalse(success);
         Assert.AreEqual(0m, result);
}

        [TestMethod]
public void TryExchange_UnknownToUnit_ReturnsFalse()
        {
            // Arrange & Act
            bool success = _converter.TryExchange("meters", "unknown_unit", 10m, out decimal result);

            // Assert
            Assert.IsFalse(success);
      Assert.AreEqual(0m, result);
        }

        [TestMethod]
  public void TryExchange_NullFromUnit_ReturnsFalse()
     {
   // Arrange & Act
            bool success = _converter.TryExchange(null, "meters", 10m, out decimal result);

  // Assert
            Assert.IsFalse(success);
          Assert.AreEqual(0m, result);
        }

        [TestMethod]
        public void TryExchange_NullToUnit_ReturnsFalse()
        {
      // Arrange & Act
       bool success = _converter.TryExchange("meters", null, 10m, out decimal result);

            // Assert
            Assert.IsFalse(success);
            Assert.AreEqual(0m, result);
 }

        [TestMethod]
        public void TryExchange_EmptyFromUnit_ReturnsFalse()
        {
     // Arrange & Act
          bool success = _converter.TryExchange("", "meters", 10m, out decimal result);

            // Assert
 Assert.IsFalse(success);
            Assert.AreEqual(0m, result);
        }

        [TestMethod]
        public void TryExchange_EmptyToUnit_ReturnsFalse()
        {
    // Arrange & Act
            bool success = _converter.TryExchange("meters", "", 10m, out decimal result);

        // Assert
            Assert.IsFalse(success);
            Assert.AreEqual(0m, result);
        }

        [TestMethod]
     public void TryExchange_ZeroValue_ReturnsZero()
      {
            // Arrange & Act
         bool success = _converter.TryExchange("meters", "kilometers", 0m, out decimal result);

            // Assert
    Assert.IsTrue(success);
    Assert.AreEqual(0m, result);
        }

        [TestMethod]
    public void TryExchange_NegativeValue_ReturnsNegativeResult()
        {
// Arrange & Act
bool success = _converter.TryExchange("meters", "kilometers", -1000m, out decimal result);

   // Assert
            Assert.IsTrue(success);
  Assert.AreEqual(-1m, result);
        }

      [TestMethod]
        public void TryExchange_VeryLargeValue_ConvertsCorrectly()
        {
     // Arrange & Act
          bool success = _converter.TryExchange("bytes", "terabytes", 1099511627776m, out decimal result);

            // Assert
     Assert.IsTrue(success);
 Assert.AreEqual(1m, result);
    }

        [TestMethod]
      public void TryExchange_DecimalPrecision_MaintainsPrecision()
      {
         // Arrange & Act
            bool success = _converter.TryExchange("meters", "feet", 1.5m, out decimal result);

     // Assert
            Assert.IsTrue(success);
     Assert.AreEqual(4.92125984252m, result, 0.00001m);
        }

        #endregion

        #region Cross-Category Incompatibility Tests

        [TestMethod]
        public void TryExchange_LengthToMass_ReturnsFalse()
        {
            // Arrange & Act
bool success = _converter.TryExchange("meters", "grams", 100m, out decimal result);

  // Assert
            Assert.IsFalse(success);
        }

  [TestMethod]
     public void TryExchange_TemperatureToVolume_ReturnsFalse()
        {
            // Arrange & Act
          bool success = _converter.TryExchange("degrees_celsius", "liters", 100m, out decimal result);

    // Assert
    Assert.IsFalse(success);
        }

      [TestMethod]
        public void TryExchange_SpeedToEnergy_ReturnsFalse()
        {
      // Arrange & Act
    bool success = _converter.TryExchange("meters_per_second", "joules", 100m, out decimal result);

       // Assert
    Assert.IsFalse(success);
  }

        #endregion

        #region Enhanced Length Units Tests

        [TestMethod]
        public void TryExchange_NanometersToMeters_ReturnsCorrectValue()
        {
            // Arrange & Act
            bool success = _converter.TryExchange("nanometers", "meters", 1000000000m, out decimal result);

            // Assert
            Assert.IsTrue(success);
            Assert.AreEqual(1m, result);
        }

        [TestMethod]
        public void TryExchange_AngstromToNanometers_ReturnsCorrectValue()
        {
            // Arrange & Act
            bool success = _converter.TryExchange("angstrom", "nanometers", 10m, out decimal result);

            // Assert
            Assert.IsTrue(success);
            Assert.AreEqual(1m, result);
        }

        [TestMethod]
        public void TryExchange_MicrometersToMillimeters_ReturnsCorrectValue()
        {
            // Arrange & Act
            bool success = _converter.TryExchange("micrometers", "millimeters", 1000m, out decimal result);

            // Assert
            Assert.IsTrue(success);
            Assert.AreEqual(1m, result);
        }

        #endregion

        #region Enhanced Mass Units Tests

        [TestMethod]
        public void TryExchange_MicrogramsToGrams_ReturnsCorrectValue()
        {
            // Arrange & Act
            bool success = _converter.TryExchange("micrograms", "grams", 1000000m, out decimal result);

            // Assert
            Assert.IsTrue(success);
            Assert.AreEqual(1m, result);
        }

        [TestMethod]
        public void TryExchange_TonsToKilograms_ReturnsCorrectValue()
        {
            // Arrange & Act
            bool success = _converter.TryExchange("tons", "kilograms", 1m, out decimal result);

            // Assert
            Assert.IsTrue(success);
            Assert.AreEqual(907.185m, result);
        }

        [TestMethod]
        public void TryExchange_CaratsToGrams_ReturnsCorrectValue()
        {
            // Arrange & Act
            bool success = _converter.TryExchange("carats", "grams", 5m, out decimal result);

            // Assert
            Assert.IsTrue(success);
            Assert.AreEqual(1m, result);
        }

        #endregion

        #region Pressure Conversion Tests

        [TestMethod]
        public void TryExchange_BarToPascals_ReturnsCorrectValue()
        {
            // Arrange & Act
            bool success = _converter.TryExchange("bar", "pascals", 1m, out decimal result);

            // Assert
            Assert.IsTrue(success);
            Assert.AreEqual(100000m, result);
        }

        [TestMethod]
        public void TryExchange_PsiToPascals_ReturnsCorrectValue()
        {
            // Arrange & Act
            bool success = _converter.TryExchange("psi", "pascals", 1m, out decimal result);

            // Assert
            Assert.IsTrue(success);
            Assert.AreEqual(6894.76m, result);
        }

        [TestMethod]
        public void TryExchange_AtmosphereToBar_ReturnsCorrectValue()
        {
            // Arrange & Act
            bool success = _converter.TryExchange("atmosphere", "bar", 1m, out decimal result);

            // Assert
            Assert.IsTrue(success);
            Assert.AreEqual(1.01325m, result);
        }

        [TestMethod]
        public void TryExchange_TorrToPascals_ReturnsCorrectValue()
        {
            // Arrange & Act
            bool success = _converter.TryExchange("torr", "pascals", 1m, out decimal result);

            // Assert
            Assert.IsTrue(success);
            Assert.AreEqual(133.322m, result);
        }

        #endregion

        #region Time Units Tests

        [TestMethod]
        public void TryExchange_MillisecondsToSeconds_ReturnsCorrectValue()
        {
            // Arrange & Act
            bool success = _converter.TryExchange("milliseconds", "seconds", 1000m, out decimal result);

            // Assert
            Assert.IsTrue(success);
            Assert.AreEqual(1m, result);
        }

        [TestMethod]
        public void TryExchange_MinutesToSeconds_ReturnsCorrectValue()
        {
            // Arrange & Act
            bool success = _converter.TryExchange("minutes", "seconds", 1m, out decimal result);

            // Assert
            Assert.IsTrue(success);
            Assert.AreEqual(60m, result);
        }

        [TestMethod]
        public void TryExchange_HoursToMinutes_ReturnsCorrectValue()
        {
            // Arrange & Act
            bool success = _converter.TryExchange("hours", "minutes", 1m, out decimal result);

            // Assert
            Assert.IsTrue(success);
            Assert.AreEqual(60m, result);
        }

        [TestMethod]
        public void TryExchange_DaysToHours_ReturnsCorrectValue()
        {
            // Arrange & Act
            bool success = _converter.TryExchange("days", "hours", 1m, out decimal result);

            // Assert
            Assert.IsTrue(success);
            Assert.AreEqual(24m, result);
        }

        [TestMethod]
        public void TryExchange_WeeksToDays_ReturnsCorrectValue()
        {
            // Arrange & Act
            bool success = _converter.TryExchange("weeks", "days", 1m, out decimal result);

            // Assert
            Assert.IsTrue(success);
            Assert.AreEqual(7m, result);
        }

        #endregion

        #region Power Units Tests

        [TestMethod]
        public void TryExchange_KilowattsToWatts_ReturnsCorrectValue()
        {
            // Arrange & Act
            bool success = _converter.TryExchange("kilowatts", "watts", 1m, out decimal result);

            // Assert
            Assert.IsTrue(success);
            Assert.AreEqual(1000m, result);
        }

        [TestMethod]
        public void TryExchange_HorsepowerToWatts_ReturnsCorrectValue()
        {
            // Arrange & Act
            bool success = _converter.TryExchange("horsepower", "watts", 1m, out decimal result);

            // Assert
            Assert.IsTrue(success);
            Assert.AreEqual(745.7m, result);
        }

        [TestMethod]
        public void TryExchange_MilliwattsToWatts_ReturnsCorrectValue()
        {
            // Arrange & Act
            bool success = _converter.TryExchange("milliwatts", "watts", 1000m, out decimal result);

            // Assert
            Assert.IsTrue(success);
            Assert.AreEqual(1m, result);
        }

        #endregion

        #region Frequency Units Tests

        [TestMethod]
        public void TryExchange_KilohertzToHertz_ReturnsCorrectValue()
        {
            // Arrange & Act
            bool success = _converter.TryExchange("kilohertz", "hertz", 1m, out decimal result);

            // Assert
            Assert.IsTrue(success);
            Assert.AreEqual(1000m, result);
        }

        [TestMethod]
        public void TryExchange_MegahertzToKilohertz_ReturnsCorrectValue()
        {
            // Arrange & Act
            bool success = _converter.TryExchange("megahertz", "kilohertz", 1m, out decimal result);

            // Assert
            Assert.IsTrue(success);
            Assert.AreEqual(1000m, result);
        }

        [TestMethod]
        public void TryExchange_GigahertzToMegahertz_ReturnsCorrectValue()
        {
            // Arrange & Act
            bool success = _converter.TryExchange("gigahertz", "megahertz", 1m, out decimal result);

            // Assert
            Assert.IsTrue(success);
            Assert.AreEqual(1000m, result);
        }

        #endregion

        #region Angle Units Tests

        [TestMethod]
        public void TryExchange_DegreesToRadians_ReturnsCorrectValue()
        {
            // Arrange & Act
            bool success = _converter.TryExchange("degrees", "radians", 180m, out decimal result);

            // Assert
            Assert.IsTrue(success);
            Assert.AreEqual(3.14159265358979m, result, 0.0000001m);
        }

        [TestMethod]
        public void TryExchange_RadiansToDegrees_ReturnsCorrectValue()
        {
            // Arrange & Act
            bool success = _converter.TryExchange("radians", "degrees", 3.14159265358979m, out decimal result);

            // Assert
            Assert.IsTrue(success);
            Assert.AreEqual(180m, result, 0.0001m);
        }

        [TestMethod]
        public void TryExchange_ArcMinuteToDegrees_ReturnsCorrectValue()
        {
            // Arrange & Act
            bool success = _converter.TryExchange("arc_minute", "degrees", 60m, out decimal result);

            // Assert
            Assert.IsTrue(success);
            Assert.AreEqual(1m, result, 0.0001m);
        }

        #endregion

        #region Electric Current Units Tests

        [TestMethod]
        public void TryExchange_MilliampsToAmperes_ReturnsCorrectValue()
        {
            // Arrange & Act
            bool success = _converter.TryExchange("milliamps", "amperes", 1000m, out decimal result);

            // Assert
            Assert.IsTrue(success);
            Assert.AreEqual(1m, result);
        }

        [TestMethod]
        public void TryExchange_MicroampsToMilliamps_ReturnsCorrectValue()
        {
            // Arrange & Act
            bool success = _converter.TryExchange("microamps", "milliamps", 1000m, out decimal result);

            // Assert
            Assert.IsTrue(success);
            Assert.AreEqual(1m, result);
        }

        #endregion

        #region Voltage Units Tests

        [TestMethod]
        public void TryExchange_MillivoltsToVolts_ReturnsCorrectValue()
        {
            // Arrange & Act
            bool success = _converter.TryExchange("millivolts", "volts", 1000m, out decimal result);

            // Assert
            Assert.IsTrue(success);
            Assert.AreEqual(1m, result);
        }

        [TestMethod]
        public void TryExchange_KilovoltsToVolts_ReturnsCorrectValue()
        {
            // Arrange & Act
            bool success = _converter.TryExchange("kilovolts", "volts", 1m, out decimal result);

            // Assert
            Assert.IsTrue(success);
            Assert.AreEqual(1000m, result);
        }

        #endregion

        #region Electric Resistance Units Tests

        [TestMethod]
        public void TryExchange_KilohmsToOhms_ReturnsCorrectValue()
        {
            // Arrange & Act
            bool success = _converter.TryExchange("kilohms", "ohms", 1m, out decimal result);

            // Assert
            Assert.IsTrue(success);
            Assert.AreEqual(1000m, result);
        }

        [TestMethod]
        public void TryExchange_MegohmsToKilohms_ReturnsCorrectValue()
        {
            // Arrange & Act
            bool success = _converter.TryExchange("megohms", "kilohms", 1m, out decimal result);

            // Assert
            Assert.IsTrue(success);
            Assert.AreEqual(1000m, result);
        }

        [TestMethod]
        public void TryExchange_MilliohmsToOhms_ReturnsCorrectValue()
        {
            // Arrange & Act
            bool success = _converter.TryExchange("milliohms", "ohms", 1000m, out decimal result);

            // Assert
            Assert.IsTrue(success);
            Assert.AreEqual(1m, result);
        }

        #endregion

        #region Enhanced Data Storage Tests

        [TestMethod]
        public void TryExchange_BitsToBytes_ReturnsCorrectValue()
        {
            // Arrange & Act
            bool success = _converter.TryExchange("bits", "bytes", 8m, out decimal result);

            // Assert
            Assert.IsTrue(success);
            Assert.AreEqual(1m, result);
        }

        [TestMethod]
        public void TryExchange_KbToBytes_ReturnsCorrectValue()
        {
            // Arrange & Act
            bool success = _converter.TryExchange("kb", "bytes", 1m, out decimal result);

            // Assert
            Assert.IsTrue(success);
            Assert.AreEqual(1024m, result);
        }

        [TestMethod]
        public void TryExchange_MbToKb_ReturnsCorrectValue()
        {
            // Arrange & Act
            bool success = _converter.TryExchange("mb", "kb", 1m, out decimal result);

            // Assert
            Assert.IsTrue(success);
            Assert.AreEqual(1024m, result);
        }

        [TestMethod]
        public void TryExchange_GbToMb_ReturnsCorrectValue()
        {
            // Arrange & Act
            bool success = _converter.TryExchange("gb", "mb", 1m, out decimal result);

            // Assert
            Assert.IsTrue(success);
            Assert.AreEqual(1024m, result);
        }

        [TestMethod]
        public void TryExchange_TbToGb_ReturnsCorrectValue()
        {
            // Arrange & Act
            bool success = _converter.TryExchange("tb", "gb", 1m, out decimal result);

            // Assert
            Assert.IsTrue(success);
            Assert.AreEqual(1024m, result);
        }

        #endregion
    }
}
