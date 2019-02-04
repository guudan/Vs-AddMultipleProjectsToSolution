# Road map

- [ ] Add projects to the solution according to the order based on project references (so the referenced project are added first and only then referncing projects), to not loose those references.
- [ ] Add exclusion list to skip folders like "obj", "bin" from searching.
- [ ] Preserve list of extensions used for searching projects between tool executions.
- [ ] Log performed operations to the Visual Studio output window.
- [ ] Turn on/off creation of solution folders on folder level.
- [ ] Add icons for known project types.
- [ ] Align style of the Tree View displaying projects hierarchy with Visual Studio.

<!--
Features that have a checkmark are complete and available for
download in the
[CI build](http://vsixgallery.com/extension/2ed01419-2b11-4128-a2ca-0adfa0fc7498/).
-->

# Change log

These are the changes to each version that has been released
on the official Visual Studio extension gallery.

## 1.0

- [x] Add mutliple projects to solution.
	- [x] Load projects from given directory (recursive search by file extension).
    - [x] Select which projects to add to the solution.
    - [x] Define if solution folders hierarchy should be also created.