
import { Location } from './location';


export interface CreateEventDTO {
    name?: string | null;
    description?: string | null;
    date?: string;
    location?: Location;
    maxParticipants?: number;
    image?: Blob | null;
    categoryId?: string;
}

