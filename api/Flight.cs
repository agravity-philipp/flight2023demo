
using System;
using Newtonsoft.Json;

public class Flight {

    [JsonProperty("id")]
    public int Id;

    [JsonProperty("from")]
    public string From;

    [JsonProperty("to")]
    public string To;

    [JsonProperty("date")]
    public DateTime Date;

}