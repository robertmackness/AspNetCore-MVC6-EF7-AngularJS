using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http;

namespace TheWorld.Services
{
    public class CoordService
    {
        private ILogger<CoordService> _logger;

        public CoordService(ILogger<CoordService> logger)
        {
            _logger = logger;
        }

        public async Task<CoordServiceResult> Lookup(string location)
        {
            var result = new CoordServiceResult()
            {
                Success = false,
                Message = "Undetermined failure while looking up coordinates"
            };

            // Lookup Coords
            var bingKey = Startup.Configuration["AppSettings:BingKey"];
            var encodedName = WebUtility.UrlEncode(location);
            var url = $"http://dev.virtualearth.net/REST/v1/Locations?query={encodedName}&output=json&key={bingKey}";

            var client = new HttpClient();
            var json = await client.GetStringAsync(url);

            // Using a GET to the url above retrieves a giant JSON object from Bing. The below code
            // has been produced to match the current (03 Apr 2016) output that Bing is providing.
            // This may entirely break if Bing changes the hierarchy or format of the returned JSON object.
            var results = JRaw.Parse(json);
            var resources = results["resourceSets"][0]["resources"];
            if (!resources.HasValues)
            {
                result.Message = $"Could not find '{location}' as location";
                return result;
            } 
            else
            {
                var confidence = (string)resources[0]["confidence"];
                if (confidence != "High")
                {
                    result.Message = $"Could not find a confident match for '{location}' as a location.";
                }
                else
                {
                    var coords = resources[0]["geocodePoints"][0]["coordinates"];
                    result.Latitude = (double)coords[0];
                    result.Longitude = (double)coords[1];
                    result.Success = true;
                    result.Message = "Success";
                }
                return result;
            }
        }
    }
}

// EXAMPLE Bing Maps Object
//{
//  "authenticationResultCode": "ValidCredentials",
//  "brandLogoUri": "http://dev.virtualearth.net/Branding/logo_powered_by.png",
//  "copyright": "Copyright © 2016 Microsoft and its suppliers. All rights reserved. This API cannot be accessed and the content and any results may not be used, reproduced or transmitted in any manner without express written permission from Microsoft Corporation.",
//  "resourceSets": [
//    {
//      "estimatedTotal": 5,
//      "resources": [
//        {
//          "__type": "Location:http://schemas.microsoft.com/search/local/ws/rest/v1",
//          "bbox": [
//            25.451837539672852,
//            -80.832115173339844,
//            26.144598007202148,
//            -79.587150573730469
//          ],
//          "name": "Miami, FL",
//          "point": {
//            "type": "Point",
//            "coordinates": [
//              25.774810791015625,
//              -80.1977310180664
//            ]
//          },
//          "address": {
//            "adminDistrict": "FL",
//            "adminDistrict2": "Miami-Dade Co.",
//            "countryRegion": "United States",
//            "formattedAddress": "Miami, FL",
//            "locality": "Miami"
//          },
//          "confidence": "High",
//          "entityType": "PopulatedPlace",
//          "geocodePoints": [
//            {
//              "type": "Point",
//              "coordinates": [
//                25.774810791015625,
//                -80.1977310180664
//              ],
//              "calculationMethod": "Rooftop",
//              "usageTypes": [
//                "Display"
//              ]
//            }
//          ],
//          "matchCodes": [
//            "Good"
//          ]
//        },
//        {
//          "__type": "Location:http://schemas.microsoft.com/search/local/ws/rest/v1",
//          "bbox": [
//            25.747062683105469,
//            -80.226043701171875,
//            25.850976943969727,
//            -80.039291381835938
//          ],
//          "name": "Miami Beach, FL",
//          "point": {
//            "type": "Point",
//            "coordinates": [
//              25.793350219726563,
//              -80.1349105834961
//            ]
//          },
//          "address": {
//            "adminDistrict": "FL",
//            "adminDistrict2": "Miami-Dade Co.",
//            "countryRegion": "United States",
//            "formattedAddress": "Miami Beach, FL",
//            "locality": "Miami Beach"
//          },
//          "confidence": "High",
//          "entityType": "PopulatedPlace",
//          "geocodePoints": [
//            {
//              "type": "Point",
//              "coordinates": [
//                25.793350219726563,
//                -80.1349105834961
//              ],
//              "calculationMethod": "Rooftop",
//              "usageTypes": [
//                "Display"
//              ]
//            }
//          ],
//          "matchCodes": [
//            "Good"
//          ]
//        },
//        {
//          "__type": "Location:http://schemas.microsoft.com/search/local/ws/rest/v1",
//          "bbox": [
//            36.827018737792969,
//            -94.97979736328125,
//            36.927467346191406,
//            -94.776618957519531
//          ],
//          "name": "Miami, OK",
//          "point": {
//            "type": "Point",
//            "coordinates": [
//              36.874038696289063,
//              -94.8775634765625
//            ]
//          },
//          "address": {
//            "adminDistrict": "OK",
//            "adminDistrict2": "Ottawa Co.",
//            "countryRegion": "United States",
//            "formattedAddress": "Miami, OK",
//            "locality": "Miami"
//          },
//          "confidence": "High",
//          "entityType": "PopulatedPlace",
//          "geocodePoints": [
//            {
//              "type": "Point",
//              "coordinates": [
//                36.874038696289063,
//                -94.8775634765625
//              ],
//              "calculationMethod": "Rooftop",
//              "usageTypes": [
//                "Display"
//              ]
//            }
//          ],
//          "matchCodes": [
//            "Good"
//          ]
//        },
//        {
//          "__type": "Location:http://schemas.microsoft.com/search/local/ws/rest/v1",
//          "bbox": [
//            33.3890495300293,
//            -110.88050079345703,
//            33.412857055664062,
//            -110.85221862792969
//          ],
//          "name": "Miami, AZ",
//          "point": {
//            "type": "Point",
//            "coordinates": [
//              33.398521423339844,
//              -110.87055206298828
//            ]
//          },
//          "address": {
//            "adminDistrict": "AZ",
//            "adminDistrict2": "Gila Co.",
//            "countryRegion": "United States",
//            "formattedAddress": "Miami, AZ",
//            "locality": "Miami"
//          },
//          "confidence": "High",
//          "entityType": "PopulatedPlace",
//          "geocodePoints": [
//            {
//              "type": "Point",
//              "coordinates": [
//                33.398521423339844,
//                -110.87055206298828
//              ],
//              "calculationMethod": "Rooftop",
//              "usageTypes": [
//                "Display"
//              ]
//            }
//          ],
//          "matchCodes": [
//            "Good"
//          ]
//        },
//        {
//          "__type": "Location:http://schemas.microsoft.com/search/local/ws/rest/v1",
//          "bbox": [
//            35.685298919677734,
//            -100.6455078125,
//            35.699836730957031,
//            -100.63021850585937
//          ],
//          "name": "Miami, TX",
//          "point": {
//            "type": "Point",
//            "coordinates": [
//              35.691818237304688,
//              -100.63854217529297
//            ]
//          },
//          "address": {
//            "adminDistrict": "TX",
//            "adminDistrict2": "Roberts Co.",
//            "countryRegion": "United States",
//            "formattedAddress": "Miami, TX",
//            "locality": "Miami"
//          },
//          "confidence": "High",
//          "entityType": "PopulatedPlace",
//          "geocodePoints": [
//            {
//              "type": "Point",
//              "coordinates": [
//                35.691818237304688,
//                -100.63854217529297
//              ],
//              "calculationMethod": "Rooftop",
//              "usageTypes": [
//                "Display"
//              ]
//            }
//          ],
//          "matchCodes": [
//            "Good"
//          ]
//        }
//      ]
//    }
//  ],
//  "statusCode": 200,
//  "statusDescription": "OK",
//  "traceId": "3aff533a95df4dfbb5baf0a726ceb9ae|DB40061020|02.00.153.3000|CO3SCH010265408, CO3SCH010265516, CO3SCH010265217"
//}