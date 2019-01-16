export class KeyValuePair<TKey, TValue> {
    value: TValue;
    key: TKey;
    constructor(key: TKey, value: TValue) {
        this.key = key;
        this.value = value;
    }
}
