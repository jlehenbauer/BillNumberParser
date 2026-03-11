# Bill Number Parser

Contains the `BillNumber` class for the purposes of 1) validating and 2) usably defining Bill Numbers.

## BillNumber Class

The `BillNumber` class defines the properties of a Bill Number:

- `BillChamber`: An enum representing the chamber of the bill, either "H" for House or "S" for Senate.
- `BillType`: An enum describing type of bill, such as "R" for Resolution, "B" for Bill, etc.
- `BillNumber`: The numeric part of the bill number, which must be a positive integer, with one to five digits (inclusive).
- `IsValid`: A boolean property that indicates whether the bill number is valid. Set during construction.

The constructor that takes a string as input performs the following validations:

- `BillChamber` must be either "H" or "S".
- `BillType` must be one of the predefined types ("R", "B", "CR", or "JR".).
- `BillNumber` must be a positive integer with one two five digits (inclusive).

If any of these validations fail, the `IsValid` property is set to `false`, and the invalid components remain unset and an ArgumentException is raised. If all validations pass, the properties are set according to the input string, and `IsValid` is set to `true`.

## Usage

Create an instance of the `BillNumber` class by passing a string representation of the bill number:
```csharp
var billNumber = new BillNumber("HJR1234");
var isValid = billNumber.IsValid; // true
```

If an instance is not needed, you can also use the static `ValidateBillNumber` method to validate a bill number string without creating an instance:
```csharp
bool isValid = BillNumber.ValidateBillNumber("SB5678"); // true
```

## Tests

Find the test project at [https://github.com/jlehenbauer/BillNumberTests](https://github.com/jlehenbauer/BillNumberTests). This project is included in the solution for BillNumberParser, but needs to be pulled separately.