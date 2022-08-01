window.onload = function () {
	/* ====== Start All Elements Variabels ======= */
	const loading = document.getElementById('loading');
	const modalTriggers = document.querySelector('.popup-trigger');
	const modalCloseTrigger = document.querySelector('.popup-modal__close');
	const bodyBlackout = document.querySelector('.body-blackout');
	const popupModal = document.getElementById('popup-modal');
	/* ====== End All Elements Variabels ======= */

	/* ====== Start Structuer Tree Nodes ======= */
	OrgChart.templates.myTemplate = Object.assign({}, OrgChart.templates.ana);
	OrgChart.templates.myTemplate.size = [140, 180];
	OrgChart.templates.myTemplate.node =
		'<rect x="0" y="0" width="140" height="180"  fill="#044B94" fill-opacity="0" stroke="none"></rect>' +
		'<circle cx="70" cy="60" r="55" fill="none" stroke="#4D4D4D"></circle>' +
		'<circle cx="70" cy="-25" r="8" fill="white" stroke-width="1" stroke="#ED9422"></circle>' +
		'<circle cx="70" cy="-25" r="3.5" fill="#ED9422"></circle>' +
		'<circle cx="70" cy="60" fill="#ffffff" r="50"></circle>' +
		'<circle stroke="#D7DBDD" stroke-width="3" fill="#D7DBDD" cx="70" cy="40" r="13"></circle>' +
		'<path d="M40,83 C40,49 100,49 100,83" stroke="#D7DBDD" stroke-width="3" fill="#D7DBDD"></path>' +
		'<circle cx="70" cy="60" r="50" fill="#FF4100" opacity="0.5"></circle>';
	OrgChart.templates.myTemplate.ripple = {
		radius: 60,
		color: '#775E3B',
		rect: { x: 10, y: 0, width: 120, height: 120 },
	};
	OrgChart.templates.myTemplate.img_0 =
		'<clipPath id="ulaImg">' +
		'<circle cx="70" cy="60" r="50"></circle>' +
		'</clipPath>' +
		'<image preserveAspectRatio="xMidYMid slice" clip-path="url(#ulaImg)" xlink:href="{val}" x="20" y="10" width="100" height="100">' +
		'</image>';
	OrgChart.templates.myTemplate.field_0 =
		'<text style="font-size: 20px;" fill="#ED9422" x="70" y="140" text-anchor="middle">{val}</text>';
	OrgChart.templates.myTemplate.field_1 =
		'<text style="font-size: 12px;" fill="#00" x="70" y="155" text-anchor="middle">{val}</text>';
	OrgChart.templates.myTemplate.field_2 =
		'<text style="font-size: 12px;" fill="#00" x="70" y="168" text-anchor="middle">{val}</text>';
	OrgChart.templates.myTemplate.field_3 =
		'<text style="font-size: 12px;" fill="#00" x="70" y="180" text-anchor="middle">{val}</text>';
	OrgChart.templates.myTemplate.link =
		'<path stroke-linejoin="round" stroke="#ED9422" stroke-width="1px" fill="none" d="M{xa},{ya} {xb},{yb} {xc},{yc} L{xd},{yd}" />';
	OrgChart.templates.myTemplate.personLevel =
		'<text style="font-size: 2.2em;" x="70" y="70" fill="#FFF" text-anchor="middle">{val}%</text>';
	OrgChart.templates.myTemplate.love =
		'<image class="svg-btn"  onclick="event.stopPropagation();" ontouchend="event.stopPropagation();"  xlink:href="/TreeNodes/img/heart-{val}.svg" x="90" y="0" height="50" width="50" style="cursor: pointer"></image>';
	OrgChart.templates.myTemplate.field_number_children =
		'<rect x="77" y="88" rx="15" ry="15" width="50" height="25" fill="#FFF" style="filter: drop-shadow(0px 0px 2px gray)"></rect>' +
		'<text fill="gray" x="100" y="107" text-anchor="middle">+{val}</text>';
	OrgChart.templates.myTemplate.plus = '';
	OrgChart.templates.myTemplate.minus = '';
	OrgChart.templates.myTemplateRoot = Object.assign(
		{},
		OrgChart.templates.myTemplate
	);
	OrgChart.templates.myTemplateRoot.node =
		'<rect x="0" y="0" width="140" height="180"  fill="#044B94" fill-opacity="0" stroke="none"></rect>' +
		'<circle cx="70" cy="60" r="55" fill="none" stroke="#4D4D4D"></circle>' +
		'<circle cx="70" cy="60" fill="#ffffff" r="50"></circle>' +
		'<circle stroke="#D7DBDD" stroke-width="3" fill="#D7DBDD" cx="70" cy="40" r="13"></circle>' +
		'<path d="M40,83 C40,49 100,49 100,83" stroke="#D7DBDD" stroke-width="3" fill="#D7DBDD"></path>' +
		'<circle cx="70" cy="60" r="50" fill="#FF4100" opacity="0.5"></circle>';
	OrgChart.templates.myTemplateRoot.personLevel = '';
	OrgChart.templates.myTemplateRoot.love = '';
	let field_template =
		'<text width="230" text-overflow="ellipsis"  style="font-size: 24px;" fill="#369dea" x="125" y="100" text-anchor="middle">{val}</text>';
	OrgChart.templates.hiddenTemplate = Object.assign({}, OrgChart.templates.ana);
	OrgChart.templates.hiddenTemplate.size = [0, 0];
	OrgChart.templates.hiddenTemplate.node = '';
	OrgChart.templates.hiddenTemplate.plus = '';
	OrgChart.templates.hiddenTemplate.minus = '';
	OrgChart.templates.hiddenTemplate.img_0 = '';
	OrgChart.templates.hiddenTemplate.personLevel = '';
	OrgChart.templates.hiddenTemplate.love = '';
	OrgChart.templates.hiddenTemplate.link = '';

	/* ====== End Structuer Tree Nodes ======= */

	/* ====== Start Options Data In Tree ========== */
	let chart = new OrgChart(document.getElementById('tree'), {
		template: 'myTemplate',
		searchFields: ['name', 'code'],
		nodeMouseClick: OrgChart.action.expandCollapse,
		levelSeparation: 50,
		siblingSeparation: 0,
		collapse: {
			level: 2,
			allChildren: true,
		},
		nodeBinding: {
			field_0: 'name',
			field_1: 'code',
			field_2: 'personalPoints',
			field_3: 'groupPoints',
			img_0: 'img',
			personLevel: 'personLevel',
			love: 'love',
			field_number_children: 'numberOfChldren',
		},
		tags: {
			root: {
				template: 'myTemplateRoot',
			},
			hiddenRoot: {
				template: 'hiddenTemplate',
			},
		},
	});
	/* ====== End Options Data In Tree =========== */

	/* ======= Start Load Data To Tree =========== */
	fetchJSONFile('/treeNodes/test.Json', function (data) {
		chart.load(data);
		chart.fit();

		// If Node Level == 21 Not Collapse
		chart.on('click', function (sender, args) {
			let childNode = sender.getNode(args.node.id).childrenIds;
			if (sender.get(args.node.id).personLevel >= 21 && childNode.length > 0) {
				let childrenAreCollapsed = sender.getNode(childNode[0]).collapsed;
				if (childrenAreCollapsed) {
					return false;
				}
			}
		});

		let filtterButton = document.querySelector('#filter-btn');
		let count = 0;
		/* ===== Start Filter Tree Node ========== */
		filtterButton.addEventListener('click', function () {
			let myCheckBoxes = document.querySelectorAll('input[name="filter"]');
			let labels = [];
			let hidden = 'hiddenRoot';
			// Get All Checked Label To Labels Array
			myCheckBoxes.forEach((box) => {
				if (box.checked) {
					labels.push(box.parentElement.dataset.value);
				}
			});

			let loveCheckBox = document.querySelector('input#love');

			data.forEach((node, index) => {
				if (index !== 0) {
					node.pid = node.save;
				}
			});

			if (labels.length > 0 && loveCheckBox.checked) {
				chart.expand(null, 'all');
				data.forEach((node, index) => {
					if (index !== 0) {
						node.pid = '1';
						node.tags.splice(node.tags.indexOf(hidden), 1);
						if (labels.indexOf(node.personLevel) >= 0 && node.love === 'fill') {
							node.tags.splice(node.tags.indexOf(hidden), 1);
						} else {
							node.tags.splice(0, 0, hidden);
						}
					}
				});
			} else if (labels.length > 0) {
				data.forEach((node, index) => {
					if (index !== 0) {
						node.tags.splice(node.tags.indexOf(hidden), 1);
						if (node.save == chart.roots[0].id) {
							let children = chart.getNode(node.id).childrenIds;
							chart.collapse(node.id, children);
							if (labels.indexOf(node.personLevel) < 0) {
								node.tags.splice(0, 0, hidden);
							} else {
								node.tags.splice(node.tags.indexOf(hidden), 1);
							}
						}
					}
				});
			} else if (loveCheckBox.checked) {
				chart.expand(null, 'all');
				data.forEach((node, index) => {
					if (index !== 0) {
						node.pid = '1';

						if (node.love !== 'fill') {
							node.tags.splice(0, 0, hidden);
						} else {
							node.tags.splice(node.tags.indexOf(hidden), 1);
						}
					}
				});
			} else {
				data.forEach((node, index) => {
					chart.expand(null, 'all');
					if (index !== 0) {
						node.tags.splice(node.tags.indexOf(hidden), 1);
					}
				});
			}

			loading.classList.add('visibile');

			let time = null;

			if (time) {
				clearTimeout(time);
			}

			time = setTimeout(function () {
				if (count <= 20) {
					filtterButton.click();
					count += 1;
				} else {
					chart.update(data);
					chart.draw();
					chart.fit();
					hideModal();
					loading.classList.remove('visibile');
					count = 0;
				}
			}, 100);
		});
		/* ===== End Filter Tree Node  =========== */

		/* ====== Start Love Button ========== */
		// function to fill and empty hart
		function createLove() {
			let node = data[this.parentElement.getAttribute('node-id') - 1];
			if (node.love == 'fill') {
				node.love = 'empty';
			} else if (node.love == 'empty') {
				node.love = 'fill';
			}
			chart.update(data);
			chart.draw();
		}

		// add Event Click On hart Button
		chart.on('redraw', function (sender) {
			let btns = document.getElementsByClassName('svg-btn');
			for (let i = 0; i < btns.length; i++) {
				btns[i].addEventListener('click', createLove, false);
			}
		});
		/* ====== Start Love Button ========== */
	});

	function fetchJSONFile(path, callback) {
		let httpRequest = new XMLHttpRequest();
		httpRequest.onreadystatechange = function () {
			if (httpRequest.readyState === 4) {
				if (httpRequest.status === 200) {
					let data = JSON.parse(httpRequest.responseText);
					if (callback) callback(data);
				}
			}
		};
		httpRequest.open('GET', path);
		httpRequest.send();
	}
	/* ======= End Load Data To Tree =========== */

	/* ======= Start Function to Get Children Count ====== */
	function iterate(c, n, args) {
		args.count += n.childrenIds.length;
		for (let i = 0; i < n.childrenIds.length; i++) {
			let node = c.getNode(n.childrenIds[i]);
			iterate(c, node, args);
		}
	}
	/* ======= End Function to Get Children Count ====== */

	// For Personal Link Page
	chart.on('field', function (sender, args) {
		if (args.name == 'name' && args.data['tags'].indexOf('hiddenRoot') == -1) {
			let name = args.data['name'];
			let link = args.data['link'];
			let text = OrgChart.wrapText(name, field_template);
			let fieldData = `<a target='_blank' href="${link}"  onclick="event.stopPropagation();"> ${text}</a>`;
			args.value = fieldData;
		}
		// to get children Count
		if (args.name == 'numberOfChldren') {
			let arg = { count: 0 };
			iterate(sender, args.node, arg);
			if (arg.count > 0) {
				args.value = arg.count;
			}
		}
	});

	/* ===================== Start Modal =================== */
	function hideModal() {
		popupModal.classList.remove('is--visible');
		bodyBlackout.classList.remove('is-blacked-out');
	}

	function showModal() {
		popupModal.classList.add('is--visible');
		bodyBlackout.classList.add('is-blacked-out');
	}

	modalTriggers.onclick = showModal;
	bodyBlackout.onclick = hideModal;
	modalCloseTrigger.onclick = hideModal;
	/* ======================== End Modal ==================== */

	/* ======== Start Fit Chart When Resize Window ======== */
	let timeout = null;
	window.addEventListener('resize', function () {
		if (timeout) {
			clearTimeout(timeout);
		}
		timeout = setTimeout(function () {
			chart.fit();
		}, 500);
	});
	/* ======== Start Fit Chart When Resize Window ======== */
};
