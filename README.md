# VLCGenerator: Generate Variable Length Code (VLC) I/O in C#
This is a tool that takes a Domain-Specific Language representing a number
to Variable Length Code (VLC) mapping and produces C# code to read/write those
VLCs at bit level. It is highly customizable, and best of all - produces C# code
that's **incredibly fast and optimized**, with zero heap allocations.

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

You can configure everything. Namespace, class name, class visibility,
the type in place of BitReader and BitWriter, and even the method to invoke
to read/write bits in the type in place of BitReader and BitWriter.
