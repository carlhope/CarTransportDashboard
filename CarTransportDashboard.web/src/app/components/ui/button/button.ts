import {Component, EventEmitter, Input, Output} from '@angular/core';

@Component({
  selector: 'app-button',
  imports: [],
  templateUrl: './button.html',
  styleUrl: './button.scss',
})
export class Button {
  @Input() label: string = '';
  @Input() variant: 'primary' | 'secondary' | 'danger' = 'primary';
  @Output() pressed = new EventEmitter<void>();
  get classes(): string {
    const base =
      'px-4 py-2 rounded-md font-medium transition-colors duration-150';

    const variants: Record<string, string> = {
      primary: 'bg-blue-600 text-white hover:bg-blue-700',
      secondary: 'bg-gray-200 text-gray-800 hover:bg-gray-300',
      danger: 'bg-red-600 text-white hover:bg-red-700'
    };

    return `${base} ${variants[this.variant]}`;
  }


}
