/**
 * EventMaster.Presentation
 * No description provided (generated by Openapi Generator https://github.com/openapitools/openapi-generator)
 *
 * The version of the OpenAPI document: 1.0
 * 
 *
 * NOTE: This class is auto generated by OpenAPI Generator (https://openapi-generator.tech).
 * https://openapi-generator.tech
 * Do not edit the class manually.
 */
import { Location } from './location';


export interface EventFilterDto { 
    name?: string | null;
    date?: string | null;
    location?: Location;
    categoryId?: string | null;
    pageNumber?: number;
    pageSize?: number;
}

