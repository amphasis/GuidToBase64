# GuidToBase64

## Overview
This utility provides convenient conversion between various representations of GUID and ObjectId values to formats suitable for use in MongoDB Compass and vice versa. The tool simplifies working with MongoDB data by automatically converting between different data formats commonly used in .NET applications and MongoDB queries.

## Features
- **Clipboard Integration**: Automatically processes clipboard content for seamless workflow
- **Bidirectional Conversion**: Converts both to and from MongoDB Compass compatible formats
- **Multiple Input Formats**: Supports various GUID and ObjectId representations
- **System Tray Access**: Quick access through system tray context menu
- **Hotkey Support**: Fast conversion using Alt+Ctrl+Z hotkey
- **Batch Processing**: Handles collections of values for bulk operations

The utility helps with converting data in the clipboard for ease of use with MongoDB Compass. Conversion occurs using the hotkey Alt+Ctrl+Z, or from the context menu in the system tray. Supported conversion options:
### GUID -> BinData

<table>
<tr>
<th> From </th>
<th> To </th>
</tr>
<tr>
<td> 7c81acd9-0eee-408a-940b-23e5b6be0054 </td>
<td> BinData(3, '2ayBfO4OikCUCyPltr4AVA==') </td>
</tr>
<tr>
<td> Some string containing 7C81ACD9-0EEE-408A-940B-23E5B6BE0054 a GUID </td>
<td> BinData(3, '2ayBfO4OikCUCyPltr4AVA==') </td>
</tr>
</table>

### BinData -> GUID

<table>
<tr>
<th> From </th>
<th> To </th>
</tr>
<tr>
<td> BinData(3, '2ayBfO4OikCUCyPltr4AVA==') </td>
<td> 7c81acd9-0eee-408a-940b-23e5b6be0054 </td>
</tr>
<tr>
<td> 2ayBfO4OikCUCyPltr4AVA== </td>
<td> 7c81acd9-0eee-408a-940b-23e5b6be0054 </td>
</tr>
</table>

### Hex -> ObjectId

<table>
<tr>
<th> From </th>
<th> To </th>
</tr>
<tr>
<td> 866a3f62014f632654f693c4 </td>
<td> ObjectId('866a3f62014f632654f693c4') </td>
</tr>
</table>

### GUID collection -> Comma separated BinData collection

<table>
<tr>
<th> From </th>
<th> To </th>
</tr>
<tr>
<td>
527fb090-15f1-45f5-9bfa-5318ea9c1acb<br/>
e4a03030-266c-4f27-adab-037e3cc2c72e<br/>
07a6449f-5e84-4acb-81ca-c1d18ebbc70c<br/>
</td>
<td>
BinData(3, 'kLB/UvEV9UWb+lMY6pwayw=='),<br/>
BinData(3, 'MDCg5GwmJ0+tqwN+PMLHLg=='),<br/>
BinData(3, 'n0SmB4Rey0qBysHRjrvHDA==')<br/>
</td>
</tr>
</table>

### BinData collection -> GUID collection

<table>
<tr>
<th> From </th>
<th> To </th>
</tr>
<tr>
<td>
kLB/UvEV9UWb+lMY6pwayw==<br/>
BinData(3, 'MDCg5GwmJ0+tqwN+PMLHLg=='),<br/>
"n0SmB4Rey0qBysHRjrvHDA=="<br/>
</td>
<td>
527fb090-15f1-45f5-9bfa-5318ea9c1acb<br/>
e4a03030-266c-4f27-adab-037e3cc2c72e<br/>
07a6449f-5e84-4acb-81ca-c1d18ebbc70c<br/>
</td>
</tr>
</table>

### Hex collection -> Comma separated ObjectId collection

<table>
<tr>
<th> From </th>
<th> To </th>
</tr>
<tr>
<td>
866a3f62014f632654f693c4<br/>
fe6a3f72014f632654f693c4<br/>
a21a3f17814f632654f693c4<br/>
</td>
<td>
ObjectId('866a3f62014f632654f693c4'),<br/>
ObjectId('fe6a3f72014f632654f693c4'),<br/>
ObjectId('a21a3f17814f632654f693c4')<br/>
</td>
</tr>
</table>
