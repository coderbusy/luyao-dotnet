# UnitConverter Documentation

## Overview

The `UnitConverter` class provides powerful unit conversion capabilities, supporting automatic conversion between various physical quantity units. The class has been enhanced with over 100 commonly used units from the amazon_units.csv file, covering 16 major categories.

## Supported Unit Categories

### 1. Length
Supports conversion from very small scales (angstrom) to very large scales (miles).

**Base Unit**: meters

**Supported Units**:
- Metric: meters, centimeters, millimeters, kilometers, decimeters, nanometers, micrometers, picometers, angstrom
- Imperial: inches, feet, yards, miles, nautical_miles

**Examples**:
```csharp
var converter = new UnitConverter();

// Inches to centimeters
if (converter.TryExchange("inches", "centimeters", 10m, out decimal result))
{
    Console.WriteLine($"10 inches = {result} cm"); // Output: 10 inches = 25.4 cm
}

// Nanometers to micrometers
if (converter.TryExchange("nanometers", "micrometers", 1000m, out result))
{
    Console.WriteLine($"1000 nm = {result} μm"); // Output: 1000 nm = 1 μm
}
```

### 2. Mass
Supports conversion from micrograms to metric tons.

**Base Unit**: kilograms

**Supported Units**:
- Metric: kilograms, grams, milligrams, micrograms, metric_tons
- Imperial: pounds, lbs, ounces, ounce, tons
- Other: carats

**Examples**:
```csharp
// Pounds to kilograms
if (converter.TryExchange("pounds", "kilograms", 10m, out decimal result))
{
    Console.WriteLine($"10 lbs = {result} kg"); // Output: 10 lbs = 4.5359237 kg
}

// Carats to grams
if (converter.TryExchange("carats", "grams", 5m, out result))
{
    Console.WriteLine($"5 carats = {result} g"); // Output: 5 carats = 1 g
}
```

### 3. Temperature
Supports conversion between Celsius, Fahrenheit, and Kelvin (requires special offset handling).

**Base Unit**: degrees_celsius

**Supported Units**: degrees_celsius, degrees_fahrenheit, kelvin

**Examples**:
```csharp
// Celsius to Fahrenheit
if (converter.TryExchange("degrees_celsius", "degrees_fahrenheit", 100m, out decimal result))
{
    Console.WriteLine($"100°C = {result}°F"); // Output: 100°C = 212°F
}

// Celsius to Kelvin
if (converter.TryExchange("degrees_celsius", "kelvin", 0m, out result))
{
    Console.WriteLine($"0°C = {result} K"); // Output: 0°C = 273.15 K
}
```

### 4. Volume
Supports metric and imperial volume unit conversions.

**Base Unit**: liters

**Supported Units**:
- Metric: liters, milliliters, centiliters, deciliters, cubic_meters, cubic_centimeters
- Imperial: gallons, fluid_ounces, cubic_feet, cubic_inches, cubic_yards, cups

**Examples**:
```csharp
// Gallons to liters
if (converter.TryExchange("gallons", "liters", 1m, out decimal result))
{
    Console.WriteLine($"1 gallon = {result} L"); // Output: 1 gallon = 3.78541 L
}
```

### 5. Speed
Supports various speed unit conversions.

**Base Unit**: meters_per_second

**Supported Units**: meters_per_second, kilometers_per_hour, miles_per_hour, feet_per_second

### 6. Area
Supports various area unit conversions.

**Base Unit**: square_meters

**Supported Units**:
- Metric: square_meters, square_centimeters, square_millimeters, hectares
- Imperial: square_inches, square_feet, square_yards, acres

### 7. Data Storage
Supports various data storage unit conversions using binary (base-1024).

**Base Unit**: bytes

**Supported Units**:
- bits, bytes
- kilobyte, kilobytes, kb
- megabyte, megabytes, mb
- gigabyte, gigabytes, gb
- terabyte, terabytes, tb
- petabyte, petabytes

**Examples**:
```csharp
// GB to MB
if (converter.TryExchange("gb", "mb", 1m, out decimal result))
{
    Console.WriteLine($"1 GB = {result} MB"); // Output: 1 GB = 1024 MB
}

// Bits to bytes
if (converter.TryExchange("bits", "bytes", 8m, out result))
{
    Console.WriteLine($"8 bits = {result} byte"); // Output: 8 bits = 1 byte
}
```

### 8. Energy
Supports various energy unit conversions.

**Base Unit**: joules

**Supported Units**: joules, kilojoules, calories, kilocalories, kilowatt_hours, btus, watt_hours

### 9. Pressure 🆕
Supports various pressure unit conversions.

**Base Unit**: pascals

**Supported Units**:
- pascals, pascal, hectopascal, kilopascal, kilopascals, megapascal, megapascals
- bar, bars, millibar, millibars
- atmosphere, atmospheres
- pounds_per_square_inch, psi
- torr

**Examples**:
```csharp
// PSI to Pascals
if (converter.TryExchange("psi", "pascals", 14.7m, out decimal result))
{
    Console.WriteLine($"14.7 psi = {result} Pa"); // Output: 14.7 psi ≈ 101325 Pa
}

// Atmosphere to bar
if (converter.TryExchange("atmosphere", "bar", 1m, out result))
{
    Console.WriteLine($"1 atm = {result} bar"); // Output: 1 atm ≈ 1.01325 bar
}
```

### 10. Time 🆕
Supports conversion from picoseconds to weeks.

**Base Unit**: seconds

**Supported Units**:
- seconds, second
- milliseconds, millisecond
- microseconds, microsecond
- nanoseconds, nanosecond
- picoseconds, picosecond
- minutes, minute
- hours, hour
- days, day
- weeks, week

**Examples**:
```csharp
// Hours to minutes
if (converter.TryExchange("hours", "minutes", 2.5m, out decimal result))
{
    Console.WriteLine($"2.5 hours = {result} minutes"); // Output: 2.5 hours = 150 minutes
}

// Milliseconds to seconds
if (converter.TryExchange("milliseconds", "seconds", 1500m, out result))
{
    Console.WriteLine($"1500 ms = {result} s"); // Output: 1500 ms = 1.5 s
}
```

### 11. Power 🆕
Supports various power unit conversions.

**Base Unit**: watts

**Supported Units**:
- watts, watt
- milliwatts, milliwatt
- microwatts, microwatt
- kilowatts, kilowatt
- megawatts, megawatt
- horsepower

**Examples**:
```csharp
// Horsepower to watts
if (converter.TryExchange("horsepower", "watts", 1m, out decimal result))
{
    Console.WriteLine($"1 hp = {result} W"); // Output: 1 hp = 745.7 W
}

// Kilowatts to horsepower
if (converter.TryExchange("kilowatts", "horsepower", 75m, out result))
{
    Console.WriteLine($"75 kW = {result} hp"); // Output: 75 kW ≈ 100.6 hp
}
```

### 12. Frequency 🆕
Supports various frequency unit conversions.

**Base Unit**: hertz

**Supported Units**: hertz, hz, kilohertz, khz, megahertz, mhz, gigahertz, ghz

**Examples**:
```csharp
// MHz to Hz
if (converter.TryExchange("megahertz", "hertz", 2.4m, out decimal result))
{
    Console.WriteLine($"2.4 MHz = {result} Hz"); // Output: 2.4 MHz = 2400000 Hz
}

// GHz to MHz
if (converter.TryExchange("ghz", "mhz", 3.6m, out result))
{
    Console.WriteLine($"3.6 GHz = {result} MHz"); // Output: 3.6 GHz = 3600 MHz
}
```

### 13. Angle 🆕
Supports various angle unit conversions.

**Base Unit**: radians

**Supported Units**:
- radians, radian
- degrees, degree
- arc_minute, arc_minutes
- arc_sec, arc_seconds

**Examples**:
```csharp
// Degrees to radians
if (converter.TryExchange("degrees", "radians", 180m, out decimal result))
{
    Console.WriteLine($"180° = {result} rad"); // Output: 180° ≈ 3.14159 rad (π)
}

// Arc minutes to degrees
if (converter.TryExchange("arc_minute", "degrees", 60m, out result))
{
    Console.WriteLine($"60' = {result}°"); // Output: 60' = 1°
}
```

### 14. Electric Current 🆕
Supports various electric current unit conversions.

**Base Unit**: amperes

**Supported Units**:
- amperes, ampere, amps, amp
- milliamps, milliamperes
- microamps, microamperes

**Examples**:
```csharp
// Milliamps to amperes
if (converter.TryExchange("milliamps", "amperes", 500m, out decimal result))
{
    Console.WriteLine($"500 mA = {result} A"); // Output: 500 mA = 0.5 A
}
```

### 15. Voltage 🆕
Supports various voltage unit conversions.

**Base Unit**: volts

**Supported Units**:
- volts, volt
- millivolts, millivolt
- kilovolts, kilovolt

**Examples**:
```csharp
// Kilovolts to volts
if (converter.TryExchange("kilovolts", "volts", 2.2m, out decimal result))
{
    Console.WriteLine($"2.2 kV = {result} V"); // Output: 2.2 kV = 2200 V
}
```

### 16. Electric Resistance 🆕
Supports various electric resistance unit conversions.

**Base Unit**: ohms

**Supported Units**:
- ohms, ohm
- milliohms, milliohm
- kilohms, kilohm
- megohms, megohm

**Examples**:
```csharp
// Kilohms to ohms
if (converter.TryExchange("kilohms", "ohms", 4.7m, out decimal result))
{
    Console.WriteLine($"4.7 kΩ = {result} Ω"); // Output: 4.7 kΩ = 4700 Ω
}
```

## Usage

### Basic Usage

```csharp
using LuYao.Globalization;

var converter = new UnitConverter();

// Use TryExchange method for unit conversion
if (converter.TryExchange("sourceUnit", "targetUnit", value, out decimal result))
{
    Console.WriteLine($"Conversion successful: {result}");
}
else
{
    Console.WriteLine("Conversion failed: units incompatible or not found");
}
```

### Features

1. **Case Insensitive**: Unit name queries are case-insensitive
   ```csharp
   converter.TryExchange("METERS", "kilometers", 1000m, out result); // Works fine
   ```

2. **Type Safe**: Units from different categories cannot be converted to each other
   ```csharp
   converter.TryExchange("meters", "kilograms", 10m, out result); // Returns false
   ```

3. **Precise Calculation**: Uses `decimal` type to ensure high-precision calculations

4. **Same Unit Handling**: Returns original value when source and target units are the same
   ```csharp
   converter.TryExchange("meters", "meters", 100m, out result); // result = 100
   ```

## Complete Example Program

```csharp
using System;
using LuYao.Globalization;

public class Program
{
    public static void Main()
    {
        var converter = new UnitConverter();
        
        Console.WriteLine("=== Unit Conversion Examples ===\n");
        
        // 1. Length conversion
        if (converter.TryExchange("inches", "centimeters", 10m, out decimal result))
        {
            Console.WriteLine($"Length: 10 inches = {result} cm");
        }
        
        // 2. Temperature conversion
        if (converter.TryExchange("degrees_celsius", "degrees_fahrenheit", 100m, out result))
        {
            Console.WriteLine($"Temperature: 100°C = {result}°F");
        }
        
        // 3. Pressure conversion
        if (converter.TryExchange("psi", "bar", 14.7m, out result))
        {
            Console.WriteLine($"Pressure: 14.7 psi = {result:F2} bar");
        }
        
        // 4. Data storage conversion
        if (converter.TryExchange("gb", "mb", 5m, out result))
        {
            Console.WriteLine($"Storage: 5 GB = {result} MB");
        }
        
        // 5. Power conversion
        if (converter.TryExchange("horsepower", "kilowatts", 100m, out result))
        {
            Console.WriteLine($"Power: 100 hp = {result:F2} kW");
        }
        
        // 6. Frequency conversion
        if (converter.TryExchange("ghz", "mhz", 3.5m, out result))
        {
            Console.WriteLine($"Frequency: 3.5 GHz = {result} MHz");
        }
        
        // 7. Angle conversion
        if (converter.TryExchange("degrees", "radians", 90m, out result))
        {
            Console.WriteLine($"Angle: 90° = {result:F4} rad");
        }
        
        // 8. Resistance conversion
        if (converter.TryExchange("kilohms", "ohms", 10m, out result))
        {
            Console.WriteLine($"Resistance: 10 kΩ = {result} Ω");
        }
        
        // 9. Error handling: incompatible units
        if (!converter.TryExchange("meters", "kilograms", 10m, out result))
        {
            Console.WriteLine("\nError example: Cannot convert length to mass");
        }
        
        // 10. Time conversion
        if (converter.TryExchange("hours", "seconds", 2m, out result))
        {
            Console.WriteLine($"Time: 2 hours = {result} seconds");
        }
    }
}
```

**Output**:
```
=== Unit Conversion Examples ===

Length: 10 inches = 25.4 cm
Temperature: 100°C = 212°F
Pressure: 14.7 psi = 1.01 bar
Storage: 5 GB = 5120 MB
Power: 100 hp = 74.57 kW
Frequency: 3.5 GHz = 3500 MHz
Angle: 90° = 1.5708 rad
Resistance: 10 kΩ = 10000 Ω

Error example: Cannot convert length to mass
Time: 2 hours = 7200 seconds
```

## Important Notes

1. **Unit names must be exact**: Use unit names listed in the documentation. While case-insensitive, spelling must be correct
2. **Temperature conversion specifics**: Temperature conversion involves offsets (e.g., Celsius to Fahrenheit), not just simple multiplication
3. **Data storage uses binary**: KB/MB/GB use base-1024, not base-1000
4. **Precision considerations**: Some conversions involve irrational numbers (like π), with results limited by precision

## Data Source

This unit converter was developed based on the `amazon_units.csv` file (containing 786 units, of which 433 are simply convertible), ensuring comprehensive unit coverage and accurate conversion factors.

## Version History

- **v2.0** (Current): Added 8 new unit categories, 100+ new units, comprehensively enhanced conversion capabilities
- **v1.0**: Initial version, supporting 8 basic categories

## License

This project follows the MIT License.
