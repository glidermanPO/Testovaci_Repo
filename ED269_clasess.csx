using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Generic;

//CLASSES

abstract class UASZone
{
    //PROPERTIES
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public string Identifier {get; set;}
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public string Country {get; set;} = "SVK";
}

class UASZoneVersion: UASZone
{
    //PROPERTIES
    public string Name {get; set;}
    public CodeZoneType Type {get; set;}
    public CodeRestrictionType Restriction {get; set;}
    public string RestrictionConditions {get; set;}
    public int? Region {get; set;}
    public CodeZoneReasonType[] Reason {get; set;}
    public string OtherReasonInfo {get; set;}
    public CodeYesNoType RegulationExemption{get; set;}
    public string USpaceClass {get; set;}
    public string Message {get; set;}
    public string AdditionalProperties {get; set;}
    public AirspaceVolume Geometry {get; set;}
}

class AirspaceVolume
{
    //PROPERTIES
    public UomDistance UomDimensions {get; set;}
    public int? LowerLimit {get; set;}
    public CodeVerticalReferenceType LowerVerticalReference {get; set;}
    public int? UpperLimit {get; set;}
    public CodeVerticalReferenceType UpperVerticalReference {get; set;}
    public GeoShapeType HorizontalProjection {get; set;}
}

class GeoShapeType
{
    //PROPERTIES
    public string Type {get; set;}

    //CONSTRUCTOR
    public GeoShapeType() {}
}


class PolygonType: GeoShapeType
{
    //PROPERTIES
    public enum CodeType {Polygon}
}

abstract class Position
{
    [JsonPropertyName("coordinates")]
    public double[] Coordinates {get; set;}
    
    //CONSTRUCTORS
    public Position() {}
    
    public Position(double x, double y): this()
    {
        this.Coordinates = new double[] {x, y};
    }

    public Position(double x, double y, double z): this(x, y)
    {
        this.Coordinates = new double[] {x, y, z};
    }

}

class Point: Position
{
    [JsonIgnore]
    public string Name {get; set;}
    [JsonIgnore]
    public int MinItems {get;} = 2;
    [JsonIgnore]
    public int MaxItems {get;} = 2;

    //CONSTRUCTORS

    //no parameter
    public Point() {}
    //2 coordinates only
    public Point(double x, double y): base(x,y) {}

    //all coordinates only
    public Point(double x, double y, double z): base(x,y,z) {}

    //name only
    public Point(string name)
    {
        Name = name;
    }

    //name and 2 coordinates
    public Point(string name, double x, double y): base(x, y)
    {
        Name = name;
    }

    //all parameters
    public Point(string name, double x, double y, double z): base(x, y, z)
    {
        Name = name;
    }
}

class LineString
{
    //PROPERTIES
    [JsonIgnore]
    public string Name {get; set; }

    //podla predpisu GJSON musi mat koordinaty v poradi LON, LAT
    [JsonPropertyName("coordinates")]
    public List<double[]> Positions {get; set;}

    [JsonIgnore]
    public int MinItems {get;} = 2;

    //CONSTRUCTORS
    public LineString()
    {
        Positions = new List<double[]>();
    }

    public LineString(string name): this()
    {
        Name = name;
    }

    public LineString(List<double[]> positions): this()
    {
        Positions = positions;
    }

    public LineString(string name, List<double[]> positions): this(name)
    {
        Positions = positions;
    }

    //METHODS
    public void AddPosition(double x, double y)
    {
        Positions.Add(new double[] {x, y});
    }

    public void AddPosition(double x, double y, double z)
    {
        Positions.Add(new double[] {x, y, z});
    }

    public void AddPosition(double[] xyz)
    {
        Positions.Add(xyz);
    }

    public bool IsClosed()
    {
        return IsEqualPosition(Positions[0], Positions[^1]) ? true:false;
    }
}



class Polygon
{
    //PROPERTIES
    [JsonIgnore]
    public string Name {get; set; }
    
    //podla predpisu je konvencia taka, ze polygon sa moze skladat z prave jedneho vonkajsieho a 0 alebo viac vnutornych LineString-ov,
    // vonkajsi musi byt pri pohlade zhora orientovany anti-clockwise a vsetky vnutorne clockwise. 
    [JsonPropertyName("coordinates")]
    public List<List<double[]>> LineStrings {get; set;}

    [JsonIgnore]
    public int MinItems {get;} = 4;

    [JsonIgnore]
    public bool IsRing {get; private set;}

    //CONSTRUCTORS
    public Polygon()
    {
        LineStrings = new List<List<double[]>>();
    }

    public Polygon(string name): this()
    {
        Name = name;
    }

    public Polygon(string name, List<List<double[]>> lineStrings): this(name)
    {
        LineStrings = lineStrings;
    }


    // //METHODS
    // public void AddLineString(List<double[]> listOfDoubles)
    // {
    //     LineStrings.Add(listOfDoubles);
    //     if (LineStrings.Count == 4 && )
    //     {

    //     }


    //     IsClosed = IsEqualPosition(Positions[0], Positions[^1]);
    // }

    // public void AddPosition(double x, double y, double z)
    // {
    //     Positions.Add(new double[] {x, y, z});
    //     IsClosed = IsEqualPosition(Positions[0], Positions[^1]);
    // }

    // public void AddPosition(double[] xyz)
    // {
    //     Positions.Add(xyz);
    //     IsClosed = IsEqualPosition(Positions[0], Positions[^1]);
    // }

}


//ENUMS
public enum CodeZoneReasonType {AIR_TRAFFIC, SENSITIVE, PRIVACY, POPULATION, NATURE, NOISE, FOREIGN_TERRITORY, EMERGENCY, OTHER}
public enum CodeZoneType {COMMON, CUSTOMIED}
public enum CodeRestrictionType { PROHIBITED, REQ_AUTHORIZATION, CONDITIONAL, NO_RESTRICTION}
public enum CodeYesNoType {YES, NO};
public enum CodeVerticalReferenceType {AGL, AMSL};
public enum UomDistance {M, FT};
public enum CodeWeekDayType {MON, TUE, WED, THU, FRI, SAT, SUN, ANY};
public enum CodeAuthorityRole {AUTHORIZATION, NOTIFICATION, INFORMATION};
public enum CodeLatLon {Lat, Lon};

//METHODS

public static bool IsEqualPosition(double[] position01, double[] position02, bool checkAllCoordinates = false)
{
    if (!checkAllCoordinates)
    {
        return (position01[0] == position02[0] && position01[1] == position02[1]);
    }
    else
    {
        return (position01[0] == position02[0] && position01[1] == position02[1] && position01[2] == position02[2]);
    }
}

public static List<double[]> SplitToListOfArrays (string stringToSplit, string inArrayOrder = "1-0", string separator = " ")
{
    string[] simpleArray = stringToSplit.Split(separator);
    List<double[]> listOfArrays = new List<double[]>();

    switch (inArrayOrder)
    {
        case "0":
            for (int i=0; i < simpleArray.Length; i = i + 1)
            {
                listOfArrays.Add(new double[] {Convert.ToDouble(simpleArray[i])});
            }
            break;
        case "0-1":
            for (int i=0; i < simpleArray.Length - 1; i = i + 2)
            {
                listOfArrays.Add(new double[] {Convert.ToDouble(simpleArray[i]), Convert.ToDouble(simpleArray[i+1])});
            }
            break;
        case "1-0":
            for (int i=0; i < simpleArray.Length - 1; i = i + 2)
            {
                listOfArrays.Add(new double[] {Convert.ToDouble(simpleArray[i+1]), Convert.ToDouble(simpleArray[i])});
            }
            break;
        case "1-0-2":
            for (int i=0; i < simpleArray.Length - 2; i = i + 3)
            {
                listOfArrays.Add(new double[] {Convert.ToDouble(simpleArray[i+1]), Convert.ToDouble(simpleArray[i]), Convert.ToDouble(simpleArray[i+2])});
            }
            break;
        case "0-1-2":
            for (int i=0; i < simpleArray.Length - 2; i = i + 3)
            {
                listOfArrays.Add(new double[] {Convert.ToDouble(simpleArray[i]), Convert.ToDouble(simpleArray[i+1]), Convert.ToDouble(simpleArray[i+2])});
            }
            break;
    }
    return listOfArrays;
}

//CODE
UASZoneVersion UZ001 = new UASZoneVersion();
UZ001.Identifier = "IDJSDJHF";
UZ001.Reason = new CodeZoneReasonType [] {CodeZoneReasonType.AIR_TRAFFIC, CodeZoneReasonType.NATURE};

JsonSerializerOptions options = new()
{
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
};

//DATA
string posList01 = "49.01542804 21.02371216 315 48.79698591 21.16928101 325 48.78974838 21.53457642 360";
string posList02 = "49.06764012 21.36978149 48.9811904 21.56204224 48.86749717 21.47689819 48.87111046 21.22421265 48.98299297 21.14730835 49.06764012 21.36978149";

Point pt01 = new Point("Point01",2.5, 3.6, 365);

//LineString vytvoreny individualnym pridanim pozicii
LineString ls01 = new LineString("LineString01");
ls01.AddPosition(pt01.Coordinates);
ls01.AddPosition(48.5, 21.3, 315);
ls01.AddPosition(48.7, 21.8, 156);
ls01.AddPosition(2.500, 3.6, 183);

//LineString vytvoreny z AIXM position posList
LineString ls02 = new LineString("LineString02", SplitToListOfArrays(posList01, "1-0-2"));
LineString ls03 = new LineString("LineString03", SplitToListOfArrays(posList02));

//LineString vytvoreny prevzatim ineho LineString-u
LineString ls04 = new LineString("LineString04", ls03.Positions);


//OUTPUT
Console.WriteLine($"Point: '{pt01.Name}', {JsonSerializer.Serialize<Point>(pt01)}");
Console.WriteLine($"LineString: '{ls01.Name}', IsClosed = {ls01.IsClosed()}, {JsonSerializer.Serialize<LineString>(ls01)}");
Console.WriteLine($"LineString: '{ls02.Name}', IsClosed = {ls02.IsClosed()}, {JsonSerializer.Serialize<LineString>(ls02)}");
Console.WriteLine($"LineString: '{ls03.Name}', IsClosed = {ls03.IsClosed()}, {JsonSerializer.Serialize<LineString>(ls03)}");
Console.WriteLine($"LineString: '{ls04.Name}', IsClosed = {ls04.IsClosed()}, {JsonSerializer.Serialize<LineString>(ls04)}");


// Console.WriteLine($"Description: {UZ001}");
// Console.WriteLine($"Identifier: {UZ001.Identifier}");
// Console.WriteLine($"Country: {UZ001.Country}");
// Console.WriteLine($"Reason: {UZ001.Reason[1]}");
//Console.WriteLine(strJson);

