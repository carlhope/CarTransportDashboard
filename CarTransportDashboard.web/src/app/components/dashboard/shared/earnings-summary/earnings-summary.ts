import {Component, Input} from '@angular/core';
import {CurrencyPipe, DecimalPipe} from '@angular/common';
import {DurationPipe} from '../../../../pipes/duration/duration-pipe';
import {EarningsSummary as EarningsData} from '../../../../models/Earnings';

@Component({
  selector: 'app-earnings-summary',
  imports: [CurrencyPipe, DecimalPipe, DurationPipe],
  templateUrl: './earnings-summary.html',
  styleUrl: './earnings-summary.scss'
})
export class EarningsSummary {
  @Input() earnings!: EarningsData;
  currencyCode: string = 'GBP';
}
