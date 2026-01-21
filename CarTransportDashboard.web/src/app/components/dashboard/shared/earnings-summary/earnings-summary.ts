import {Component, Input} from '@angular/core';
import {CurrencyPipe} from '@angular/common';
import {EarningsSummary as EarningsData} from '../../../../models/Earnings';

@Component({
  selector: 'app-earnings-summary',
  imports: [CurrencyPipe],
  templateUrl: './earnings-summary.html',
  styleUrl: './earnings-summary.scss'
})
export class EarningsSummary {
  @Input() earnings!: EarningsData;
  currencyCode: string = 'GBP';
}
