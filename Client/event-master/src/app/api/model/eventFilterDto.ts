import { Location } from './location';


export interface EventFilterDto {
    name?: string | null;
    date?: string | null;
    location?: Location;
    categoryId?: string | null;
    pageNumber?: number;
    pageSize?: number;
}

