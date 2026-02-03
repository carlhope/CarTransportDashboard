import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'duration'
})
export class DurationPipe implements PipeTransform {

  transform(value: string | null | undefined): string {
    if (!value) return '';

    // Expecting format "HH:mm:ss"
    const [hours, minutes, seconds]:number[] = value.split(':').map(Number);

    const parts: string[] = [];
    if (hours) parts.push(`${hours} hour${hours !== 1 ? 's' : ''}`);
    if (minutes) parts.push(`${minutes} minute${minutes !== 1 ? 's' : ''}`);

    return parts.join(' ');
  }

}
