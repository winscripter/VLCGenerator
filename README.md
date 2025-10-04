# VLCGenerator: Generate Variable Length Code Table I/O in C#
This is a tool that takes a Domain-Specific Language representing a number
to Variable Length Code (VLC) table and produces C# code to read/write those
VLCs at bit level. It is highly customizable, and best of all - produces C# code
that's **incredibly fast and optimized**, with zero allocations.

For example, with this razor-like syntax:
```
@namespace MyCompany.MyProduct.VlcSerializers
@class SampleVlcSerializer
@visibility public
@readerType BitReader
@readBitMethod Read
@writerType BitWriter
@writeBitMethod Write

0 0
1 10
2 110
3 11110
4 11100
5 11101
6 11111
```
It will now generate for you C# code that represents a class SampleVlcSerializer in
namespace `MyCompany.MyProduct.VlcSerializers`. Inside, there are two static methods:
- Decode(BitReader reader) -> long
- Encode(BitWriter, long) -> void

If you invoke Decode and the next, say, 5 bits of the input BitReader (or any other input type specified in the `@readerType` option) are equal to, say, `11100`,
this method will consume those 5 bits and return the number 4.

If you invoke Encode with your BitWriter (or any other input type specified in the `@writerType` option) and a number 5, this method will encode the bits `11101` accordingly.

You can configure everything. Namespace, class name, class visibility,
the type in place of BitReader and BitWriter, and even the method to invoke
to read/write bits in the type in place of BitReader and BitWriter.

# Syntax
The syntax is very simple. It is Razor-like syntax.

Files with this syntax should be saved under the `.vlcdef` extension.

## Code definitions
Those define the VLC. Prior to the first whitespace is the value of the VLC, and anything afterwards are the bits to encode it. Example:
```
1 101
2 110
3 011
4 001
5 010
```
Bits required to encode the VLC can also have spaces in them too. They're optional, but can be used to, for instance, separate bits for readability.

```
1 1011 0100
2 0101 1000
3 1001 0011
```
Those are equivalent to:
```
1 10110100
2 01011000
3 10010011
```

## Options
These let you configure the generated code output.

They start with the '@' character.

`@namespace` lets you configure the namespace where the generated VLC serializer class will be put in the generated code.
```
@namespace MyCompany.MyProduct.VlcSerializers
```

`@class` lets you configure the name of the VLC serializer class.
```
@class MyVlcSerializer
```

`@visibility` lets you configure the access modifier of the VLC serializer class (`MyVlcSerializer`). Defaults to `internal`, but can be changed to be `public`.
```
@visibility public
```

`@readerType` specifies the type of the first parameter that the `Decode` method in the `MyVlcSerializer` class takes. For example, with this:
```
@readerType MyCompany.Utilities.BitStream
```
the `Decode` method will look like:
```cs
public static long Decode(MyCompany.Utilities.BitStream reader)
```

Similarly, `readBitMethod` specifies the name of the method to invoke inside the `reader` parameter in order to read a single bit.

The `writerType` and `writeBitMethod` is same as `readerType` and `readBitMethod`, but specific to the `Encode` method and, writing instead of reading.

# In progress
We're currently in the process of adding `VLCGenerator` to NuGet so anyone can use it. Once adding
`VLCGenerator` to NuGet is done, we'll document on how to actually generate the code from the VLC generator syntax. Stay tuned!

# Use cases
This generator should be a lifesaver when working on implementing decoders and encoders of specific
file formats and video codecs in C#. Examples:
- 🔉 MP3 and AAC include VLCs for Huffman coding
- 🔉 Dolby AC-3/E-AC-3 use VLCs in Exponent and Mantissa coding
- 🖼️ JPEG uses Huffman coding which includes VLCs
- 📽️ ITU-T H.262 includes many VLCs for macro-block types
- 📽️ ITU-T H.264's CABAC and CAVLC include VLCs for mb_type and coeff_token, respectively

<hr />

# License
MIT License

Copyright (c) 2023-2025, winscripter

See LICENSE.txt for license details.

<hr />

Made with .NET 8 and Visual Studio 2026.

Made by winscripter.
