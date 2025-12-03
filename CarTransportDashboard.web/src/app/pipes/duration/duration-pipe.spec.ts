import { DurationPipe } from './duration-pipe';

describe('DurationPipeCreatesAnInstance', () => {
  it('create an instance', () => {
    const pipe = new DurationPipe();
    expect(pipe).toBeTruthy();
  });
});

describe('DurationPipe', () => {
  let pipe: DurationPipe;

  beforeEach(() => {
    pipe = new DurationPipe();
  });

  it('should return empty string for null or undefined', () => {
    expect(pipe.transform(null)).toBe('');
    expect(pipe.transform(undefined)).toBe('');
  });

  it('should format hours and minutes correctly', () => {
    expect(pipe.transform('03:30:00')).toBe('3 hours 30 minutes');
    expect(pipe.transform('01:05:00')).toBe('1 hour 5 minutes');
  });

  it('should handle only hours', () => {
    expect(pipe.transform('02:00:00')).toBe('2 hours');
    expect(pipe.transform('01:00:00')).toBe('1 hour');
  });

  it('should handle only minutes', () => {
    expect(pipe.transform('00:45:00')).toBe('45 minutes');
    expect(pipe.transform('00:01:00')).toBe('1 minute');
  });

  it('should handle zero duration', () => {
    expect(pipe.transform('00:00:00')).toBe('');
  });
  it('should ignore seconds when formatting', () => {
    expect(pipe.transform('01:30:45')).toBe('1 hour 30 minutes');
    expect(pipe.transform('00:45:59')).toBe('45 minutes');
  });

  it('should handle seconds-only values gracefully', () => {
    expect(pipe.transform('00:00:30')).toBe('');
    expect(pipe.transform('00:00:59')).toBe('');
  });

  it('should handle mixed values with seconds', () => {
    expect(pipe.transform('02:15:10')).toBe('2 hours 15 minutes');
  });
});
