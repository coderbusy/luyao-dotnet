# SizeHelper Documentation

## Overview

The `SizeHelper` class provides functionality to extract size information from strings and convert all size values to centimeters (CM). The class supports various unit formats, mixed units, and complex size expressions.

## Features

- **Multiple Unit Support**: INCH, IN, MM, CM, DM, M
- **Flexible Separators**: Supports `x` and `*` as size separators
- **Decimal Support**: Supports decimal inputs like 10.5cm
- **Mixed Units**: Supports different units in the same expression
- **Multiple Groups**: Supports extracting multiple size groups from parentheses
- **Smart Filtering**: Automatically ignores unsupported text content
- **Unified Output**: All results are converted to centimeters (CM)

## Supported Units

| Unit | Description | Conversion Rate (relative to CM) |
|------|-------------|----------------------------------|
| CM   | Centimeter (base unit) | 1 |
| MM   | Millimeter | 1 mm = 0.1 cm |
| DM   | Decimeter | 1 dm = 10 cm |
| M    | Meter | 1 m = 100 cm |
| INCH, IN | Inch | 1 inch = 2.54 cm |

## Usage

### Basic Usage

```csharp
using LuYao.Globalization;

// Single unit format
if (SizeHelper.ExtractSize("10x10x10cm", out decimal[] result))
{
    Console.WriteLine($"Number of sizes: {result.Length}");
    foreach (var size in result)
    {
        Console.WriteLine($"{size} cm");
    }
}
// Output:
// Number of sizes: 3
// 10 cm
// 10 cm
// 10 cm
```

### No Unit (Defaults to Centimeters Only with Separators)

```csharp
// When using separators (x or *) without explicit units, defaults to centimeters
if (SizeHelper.ExtractSize("10x20x30", out decimal[] result))
{
    // result = [10, 20, 30]
    Console.WriteLine($"Length: {result[0]}cm, Width: {result[1]}cm, Height: {result[2]}cm");
}
// Output: Length: 10cm, Width: 20cm, Height: 30cm

// Single value must have a unit, otherwise parsing fails
if (!SizeHelper.ExtractSize("10", out decimal[] result2))
{
    Console.WriteLine("Parsing failed: single value must have a unit");
}
```

### Multiple Uniform Units

```csharp
// Each size value has the same unit
if (SizeHelper.ExtractSize("10cmx20cmx30cm", out decimal[] result))
{
    // result = [10, 20, 30]
}
```

### Multiple Non-Uniform Units

```csharp
// Each size value uses a different unit
if (SizeHelper.ExtractSize("10cmx5inx10m", out decimal[] result))
{
    // result = [10, 12.7, 1000]
    Console.WriteLine($"10cm = {result[0]}cm");
    Console.WriteLine($"5inch = {result[1]}cm");
    Console.WriteLine($"10m = {result[2]}cm");
}
// Output:
// 10cm = 10cm
// 5inch = 12.7cm
// 10m = 1000cm
```

### Decimal Input

```csharp
// Supports decimal precision
if (SizeHelper.ExtractSize("10.5x20.3x30.7cm", out decimal[] result))
{
    // result = [10.5, 20.3, 30.7]
}
```

### Multiple Groups with Different Units

```csharp
// Use parentheses to separate multiple size groups, each can use different units
if (SizeHelper.ExtractSize("10cmx10cmx10cm(3.94x3.94x3.94in)", out decimal[] result))
{
    // result = [10, 10, 10, 10.0076, 10.0076, 10.0076]
    Console.WriteLine($"First group (cm): {result[0]}, {result[1]}, {result[2]}");
    Console.WriteLine($"Second group (inch to cm): {result[3]}, {result[4]}, {result[5]}");
}
// Output:
// First group (cm): 10, 10, 10
// Second group (inch to cm): 10.0076, 10.0076, 10.0076
```

### Ignore Unsupported Text

```csharp
// Automatically ignores non-numeric text and extracts valid size data
if (SizeHelper.ExtractSize("尺寸(cm)：10x10x10", out decimal[] result))
{
    // result = [10, 10, 10]
    // Automatically ignores "尺寸" and "：" text
}

if (SizeHelper.ExtractSize("Product dimensions: 5*10*15 inches", out decimal[] result))
{
    // result = [12.7, 25.4, 38.1]
    // Automatically ignores "Product dimensions: " text, recognizes inches unit
}
```

### Using Asterisk Separator

```csharp
// Supports using * as separator
if (SizeHelper.ExtractSize("10*20*30cm", out decimal[] result))
{
    // result = [10, 20, 30]
}
```

### Different Unit Examples

```csharp
// Millimeter unit
if (SizeHelper.ExtractSize("100x100x100mm", out decimal[] result))
{
    // result = [10, 10, 10]  (100mm = 10cm)
}

// Decimeter unit
if (SizeHelper.ExtractSize("1x2x3dm", out decimal[] result))
{
    // result = [10, 20, 30]  (1dm = 10cm)
}

// Meter unit
if (SizeHelper.ExtractSize("1x2x3m", out decimal[] result))
{
    // result = [100, 200, 300]  (1m = 100cm)
}

// Inch unit
if (SizeHelper.ExtractSize("10x10x10in", out decimal[] result))
{
    // result = [25.4, 25.4, 25.4]  (1in = 2.54cm)
}
```

## Error Handling

```csharp
// Empty string or null
if (!SizeHelper.ExtractSize("", out decimal[] result))
{
    Console.WriteLine("Extraction failed");
    // result = []
}

// Single value must have a unit
if (SizeHelper.ExtractSize("10cm", out decimal[] result))
{
    Console.WriteLine("Single value with unit: success");
    // result = [10]
}

if (!SizeHelper.ExtractSize("10", out decimal[] result2))
{
    Console.WriteLine("Single value without unit: failed");
    // result2 = []
}

// Single value cannot have other extraneous characters
if (!SizeHelper.ExtractSize("5m1", out decimal[] result3))
{
    Console.WriteLine("Characters after unit: failed");
    // result3 = []
}

if (!SizeHelper.ExtractSize("1109020P3060", out decimal[] result4))
{
    Console.WriteLine("Complex string without separator: failed");
    // result4 = []
}

// Only text
if (!SizeHelper.ExtractSize("abc x def", out decimal[] result5))
{
    Console.WriteLine("Extraction failed or returns empty array");
    // result = [] or returns false
}
```

## Real-World Use Cases

### E-commerce Product Size Parsing

```csharp
string productDescription = "Package dimensions: 30x20x15cm (11.8x7.9x5.9in)";
if (SizeHelper.ExtractSize(productDescription, out decimal[] sizes))
{
    Console.WriteLine($"CM sizes: {sizes[0]} x {sizes[1]} x {sizes[2]}");
    Console.WriteLine($"Inch to CM: {sizes[3]:F2} x {sizes[4]:F2} x {sizes[5]:F2}");
    
    // Calculate volume (cubic centimeters)
    decimal volume = sizes[0] * sizes[1] * sizes[2];
    Console.WriteLine($"Volume: {volume} cm³");
}
```

### Logistics Package Size Standardization

```csharp
string[] packageInputs = 
{
    "10*20*30cm",
    "5x10x15 inches",
    "100x200x300mm",
    "Size: 1x2x3dm"
};

foreach (var input in packageInputs)
{
    if (SizeHelper.ExtractSize(input, out decimal[] sizes))
    {
        Console.WriteLine($"Input: {input}");
        Console.WriteLine($"Standardized (cm): {sizes[0]} x {sizes[1]} x {sizes[2]}");
        Console.WriteLine();
    }
}
```

### International Size Conversion

```csharp
// US customer input (inches)
string usInput = "24x18x12in";
if (SizeHelper.ExtractSize(usInput, out decimal[] sizes))
{
    Console.WriteLine("US sizes converted to metric:");
    Console.WriteLine($"Length: {sizes[0]}cm ({sizes[0] / 100}m)");
    Console.WriteLine($"Width: {sizes[1]}cm ({sizes[1] / 100}m)");
    Console.WriteLine($"Height: {sizes[2]}cm ({sizes[2] / 100}m)");
}
```

## Important Notes

1. **Return Value**: The method returns a `bool` indicating whether extraction was successful. On success, `arr` contains the converted size values; on failure, `arr` is an empty array.

2. **Unit Priority**: When multiple unit keywords appear in the string (e.g., "CM" and "M"), priority is:
   - INCH/IN > MM > DM > M > CM (only when no other unit is present)

3. **Case Insensitive**: All unit recognition is case-insensitive (`cm`, `CM`, `Cm` are all valid).

4. **Decimal Precision**: Uses `decimal` type to ensure high-precision size calculations.

5. **Parenthesis Groups**: Groups in parentheses are only processed if they contain valid separators (x or *).

6. **Whitespace Handling**: Whitespace is automatically processed and ignored.

## Performance Considerations

- Uses regular expressions for pattern matching, suitable for medium complexity strings
- For large batch processing, consider caching results or batch processing optimization
- `decimal` type provides precise calculations but is slightly slower than `double`

## Related Classes

- `UnitConverter`: Provides more general unit conversion functionality, supporting over 100 units across 16 categories
- `RmbHelper`: Provides RMB amount conversion functionality (Chinese currency)
