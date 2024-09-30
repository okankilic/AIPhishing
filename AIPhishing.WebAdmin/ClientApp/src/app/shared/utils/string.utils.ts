export function equalsIgnoringCase(source: string, toCompare: string) {
  return source.localeCompare(toCompare, undefined, { sensitivity: 'base' }) === 0;
}
