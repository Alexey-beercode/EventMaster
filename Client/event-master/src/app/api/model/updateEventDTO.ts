
import { Location } from './location';



export interface UpdateEventDTO {
  id?: string;
  name?: string | null;
  description?: string | null;
  date?: string;
  location?: Location;
  maxParticipants?: number;
  image?: Blob | null;
  categoryId?: string;
}

