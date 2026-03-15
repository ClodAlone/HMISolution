// from Anthem (renamed to Skinny)
///TODO: migrate to ASP.Net page callbacks + jQuery events
Skinny = 
{
Invoke: function (id, method, args, callBack, state) 
{
	var _o = {
		SID:id.split(":").join("_"),
		SM:method
	};
	
	jQuery.each(
		args,
		function(i,n)
		{
			_o['SA' + i] = encodeURIComponent(n);
		}
	);
	
	jQuery.post(
		Skinny_DefaultURL,
		_o,
		function(data)
		{
			callBack(Skinny.Result({responseText:data}),state);
		}
	);
},

Result: function(x) 
{
	var result = { "value": null, "error": null};
	var responseText = x.responseText;
	try 
	{
		result = eval("(" + responseText + ")");
	} 
	catch (e) 
	{
		if (responseText.length == 0) 
			result.error = "NORESPONSE";
		else 
		{
			result.error = "BADRESPONSE";
			result.responseText = responseText;
		}
	}
	return result;
}
};

ApplyStyles: function (stylesheet) {
    var self = this;
    function rule(sel, val) {
        var sels = sel.split(',');
        for (var i = 0; i < sels.length; i++) {
            var s = sels[i];
            var re = /\s/;
            var res = re.exec(s);
            if (res)
                s = s.replace(re, '_' + self.id + ' ');
            else
                s = s + '_' + self.id;
            if (stylesheet.addRule) //IE

                stylesheet.addRule(s, val);
            else if (stylesheet.insertRule) // Moz

                stylesheet.insertRule(s + '{' + val + '}',
        stylesheet.cssRules.length);
            else
                return; //opera

        }
    }
    rule('.PG', 'width:' + this.width + 'px');
    rule('.PG *', 'color:' + this.fgcolor + ';font-family:' +
       this.family + ';font-size:' + this.fontsize);
    rule('.PGH,.PGF,.PGC,.PGF2', 'border-color:' +
       this.headerfgcolor + ';background-color:' + this.bgcolor);
    rule('.PGC *', 'line-height:' + this.lineheight +
       'px;height:' + this.lineheight + 'px');
    rule('.PGC a,.PGC_OPEN,.PGC_CLOSED',
       'width:' + this.padwidth + 'px');
    rule('.PGC_HEAD span', 'color:' + this.headerfgcolor);
    rule('.PGI_NONE,.PGI_OPEN,.PGI_CLOSED', 'width:' +
       this.padwidth + 'px;height:' + this.LineHeightMargin() + 'px');
    rule('.PGI_NAME,.PGI_VALUE,.PGI_NAME_SUB', 'width:' +
       this.HalfWidth() + 'px;background-color:' + this.itembgcolor);
    rule('.PGI_VALUE a,.PGI_VALUE select', 'width:100%');
    rule('.PGI_NAME_SUB span', 'margin-left:' + this.padwidth + 'px');
    rule('.PGI_VALUE a:hover', 'background-color:' + this.selcolor);
    rule('.PGI_VALUE input', 'width:' + this.HalfWidthLess3() +
    'px;line-height:' + this.InputLineHeight() +
    'px;height:' + this.InputLineHeight() + 'px');
}

// start PropertyGrid

(function($) {
	$.fn.PropertyGrid = function(settings) {
		PGPATH = settings.path;
		
		settings = $.extend({
			UNFRESH_GIF: 'unfresh.gif',
			REFRESH_GIF: 'refresh.gif',
			HELPOFF_GIF: 'helpoff.gif',
			HELP_GIF:	 'help.gif',
			ON_GIF:		 'on.gif',
			OFF_GIF:	 'off.gif'
		}, settings || {});
		
		return this.each(function(){
			
			var currentedit = null;
			var endeditlock = null;
			var timerid     = null;
			var livemode    = false;
			
			var _pg = $(this);
			//deduce 'id' from itself
			var _id = _pg.attr('id');
			
			var _cfg = $.extend({
				id:_id,
				//deduce items' ids from own content
				items:$.map(
					$('.PGI_VALUE [id^="'+_id+'"]', _pg).get(),
					function(i){
						var _val = $(i).attr('id');
						var _n = parseInt(_val.slice(_id.length+1));
						
						if(isNaN(_n)) {
							_n = parseInt(_val);
						}
						
						return _n;
					}
				),
				selcolor:'',
				itembgcolor:'',
				width:_pg.width(),
				bgcolor:'',
				bordercolor:'',
				headerfgcolor:'',
				lineheight:0,
				padwidth:18,
				fgcolor:'',
				family:'',
				fontsize:'',
				interval:0
			}, settings);
			
			var oldval = null;
			var oldlbl = null;
			
			///<methods>
			var WidthInner =       function(){return _cfg.width - 2;};
			var LineHeightMargin = function(){return _cfg.lineheight + 1;};
			var WidthLessPad =     function(){return WidthInner() - _cfg.padwidth;};
			var HalfWidth =        function(){return WidthLessPad()/2;};
			var HalfWidthLess3 =   function(){return HalfWidth() - 5;};
			var InputLineHeight =  function(){return _cfg.lineheight - 4;};
			
			var ApplyStyles = function()
			{
				$.each({
						'.PG'	:{'width':_cfg.width+'px'},
						'.PG *'	:{color:_cfg.fgcolor,'font-family':_cfg.family,'font-size':_cfg.fontsize},
						'.PGH,.PGF,.PGC,.PGF2':{'border-color:':_cfg.headerfgcolor,'background-color':_cfg.bgcolor},
						'.PGC *':{'line-height':_cfg.lineheight+'px'/*,'height':_cfg.lineheight+'px'*/},
						'.PGI *':{'height':_cfg.lineheight+'px'},
						'.PGC_OPEN,.PGC_CLOSED':{'width':_cfg.padwidth+'px', 'height':_cfg.lineheight+'px'},
						'.PGC_HEAD span':{color:_cfg.headerfgcolor},
						'.PGI_NONE,.PGI_OPEN,.PGI_CLOSED':{'width':_cfg.padwidth+'px',height:LineHeightMargin()+'px'},
						'.PGI_NAME,.PGI_VALUE,.PGI_NAME_SUB':{width:HalfWidth()+'px','background-color':_cfg.itembgcolor},
//						'.PGI_VALUE a,.PGI_VALUE select':{width:'100%'},
						'.PGI_NAME_SUB span':{'margin-left':_cfg.padwidth+'px'},
						'.PGI_VALUE a:hover':{'background-color':_cfg.selcolor},
						'.PGI_VALUE input':{width:HalfWidthLess3()+'px','line-height':InputLineHeight()+'px',height:InputLineHeight()+'px'}
					},
					function(sel,val){
						$($.map(sel.split(','),
							function(s){
								var re = /\s/;
								var res = re.exec(s);
								if(res){
									s=s.replace(re,'_'+_cfg.id+' ');
								}else{
									s+='_'+_cfg.id;
								}
								return s;
							}
						).join(',')).css(val);
					}
				);
			};
			
			var EndEdit = function(sender)
			{	
				if (endeditlock)
					return;
				
				sender = $(sender);
				ToggleActivity(true);
				endeditlock = sender;
				currentedit = null;
				sender.hide().parent().prev().css('backgroundColor', _cfg.itembgcolor);
				
				var newval = sender.val();
				
				var firstchild = sender.prev().show().children(':first');
				oldlbl = firstchild;
				oldlbl.attr('disabled', true);
				oldval = firstchild.html();
				firstchild.html(newval);
				
				Skinny.Invoke(
					_cfg.id,
					'SetValue',
					[firstchild.attr('id').substr(_cfg.id.length + 1),newval],
					function(a) {
						SetValue(a);
					});
			};
			
			var CancelEdit = function(sender)
			{
				if (currentedit == null)
					return;
				
				sender = $(sender);
				currentedit = null;
				endeditlock = sender;
				
				sender.parent().prev().css('backgroundColor', _cfg.itembgcolor);
				sender.hide().prev().show();
				UpdateDescription();
			};
			
			var ToggleActivity = function(active)
			{
				if (endeditlock)
					return;
				
				var ae = $('[id$="_active"]', _pg);
				active ? ae.show() : ae.hide();
			};
			
			var UpdateDescription = function(result)
			{
				if (result && result.error) {
					ToggleActivity(false);
					return;
				}
				
				if (result == null )
					result = {'value':['An ASP.NET PropertyGrid','(c) 2006 leppie']};
				
				$('[id$="_foot"]', _pg).html('<span style="font-weight:bold;display:block">' + result.value[0] + '</span>' + result.value[1]);
				ToggleActivity(false);
			};
			
			var GetValues = function(sender)
			{
				if (oldlbl == null && currentedit == null){
					if (sender != null){
						sender.src = _cfg.UNFRESH_GIF;
						sender.disabled = true;
					}
					if (livemode ^ sender != null){
						ToggleActivity(true);
						Skinny.Invoke(
							_cfg.id,
							'GetValues',
							[],
							function(a) { UpdateValues(a,sender); }
						);
					}
				}
			};
			
			var UpdateValues = function(result,sender)
			{
				var vals = result.value;
				if (vals.length != _cfg.items.length){
					window.location = window.location;
					return;
				}
				
				$.each(
					vals,
					function(i, n) {
						var lbl = $('#'+_cfg.id + '_' + _cfg.items[i], _pg);
						
						if (lbl.html() != n){
							lbl.html(n).parent().attr('title', n);
						}
					});
					
				if (sender != null && !livemode){
					sender.src = _cfg.REFRESH_GIF;
					sender.disabled = false;
				}else if (livemode && sender == null){
					timerid = setTimeout(function() { GetValues(); }, _cfg.interval);
				}
				
				ToggleActivity(false);
				UpdateDescription();
			};

			var SetValue = function(result)
			{
				endeditlock = null;
				if (result.error != null){
					ToggleActivity(false);
					alert(result.error);
					oldlbl.html(oldval).attr('disabled', false);
					oldlbl = oldval = null;
					UpdateDescription();
					return;
				}
				oldlbl.attr('disabled', false);
				oldlbl = oldval = null;
				UpdateValues(result, "FAKE");
			};
			///</methods>
			
			var toggleCategory = function(pgc, forceOpen){
				var _btn = pgc.children(':eq(0)');
				var _content = _btn.next().next();
				
				if(_btn.is('.PGC_OPEN') && 'undefined' == typeof(forceOpen) || forceOpen) {
					_btn.removeClass('PGC_OPEN').addClass('PGC_CLOSED');
					_content.hide('fast');
				} else {
					_btn.addClass('PGC_OPEN').removeClass('PGC_CLOSED');
					_content.show('fast');
				}
			};
			
			//Expand/Collapse Category by image
			$('.PGC_OPEN', _pg).click(function(e){
				toggleCategory($(this).parent());
			});
			
			//Expand/Collapse Category by hyperlink
			$('.PGC_HEAD a', _pg).click(function(e){
				toggleCategory($(this).parent().parent().parent());
				return false;
			});
			
			//Expand All,Collapse All and Help buttons
			$('.PGH .PGH_R:first', _pg).click(function(e){
				$('.PGC', _pg).each(function(){
					toggleCategory($(this), false);
				});
			}).next().click(function(){
				$('.PGC', _pg).each(function(){
					toggleCategory($(this), true);
				});
			}).next().click(function(){
				sender = $(this);
				var ft = $('[id$="_foot"]', _pg).parent(':first');
				var vis = ft.is(':visible');
				sender.attr('src', vis
					? _cfg.HELPOFF_GIF
					: _cfg.HELP_GIF);
				vis ? ft.hide('fast') : ft.show('fast');
			});
			
			//Toggle Live mode button
			$('.PGH > .PGH_L[alt="LIVE"]', _pg).click(function(){
				sender = $(this);
				livemode = !livemode;
				var rf = sender.next();
				
				if (livemode){
					rf.attr({'src':_cfg.UNFRESH_GIF, 'disabled':true});
					sender.attr('src', _cfg.ON_GIF);
					timerid = setTimeout(function() { GetValues(); }, _cfg.interval);
				}else{
					clearTimeout(timerid);
					sender.attr('src', _cfg.OFF_GIF);
					rf.attr({'src':_cfg.REFRESH_GIF,'disabled': false});
				}
			});
			
			$('.PGH > .PGH_L[alt="REFRESH"]', _pg).click(function(){
				GetValues(this);
			});
			
			var toggleProperty = function(pgi, forceOpen)
			{
				var _btn = pgi.children(':eq(0)');
				var _content = _btn.parent().next(':first');
				
				if(_btn.is('.PGI_OPEN') && 'undefined' == typeof(forceOpen) || forceOpen) {
					_btn.removeClass('PGI_OPEN').addClass('PGI_CLOSED');
					_content.hide('fast');
				} else {
					_btn.removeClass('PGI_CLOSED').addClass('PGI_OPEN');
					_content.show('fast');
				}
			};
			
			//Expand/collapse property
			$('.PGI_OPEN,.PGI_CLOSED', _pg).click(function(){
				toggleProperty($(this).parent());
			});
			
			//Update description
			$('.PGI > :nth-child(2)', _pg).click(function(){
				
				if (currentedit)
					EndEdit(currentedit);
				
				var s = $(this).next().get(0).firstChild.firstChild;
				ToggleActivity(true);
				
				Skinny.Invoke(
					_cfg.id,
					'GetDescription',
					[s.id.substr(_cfg.id.length + 1)], 
					function(a) {
						UpdateDescription(a);
					});
				return false;
			});
			
			//Open author's blog
			$('img[alt="INFO"]', _pg).click(function(){
				open('http://blogs.wdevs.com/leppie', '_blank');
			})
			
			//begin edit
			$('.PGI_VALUE a[title="Click to edit"]', _pg).click(function(){
				if (currentedit){
					EndEdit(currentedit);
				}
				
				sender = $(this);
				endeditlock = null;
				var s = sender.children(':first');
				ToggleActivity(true);
				
				Skinny.Invoke(
					_cfg.id,
					'GetDescription',
					[s.attr('id').substr(_cfg.id.length + 1)],
					function(a) { UpdateDescription(a); });
				
				sender.hide().parent().prev().css('backgroundColor', _cfg.selcolor);
				currentedit = sender.next().show().val(s.html()).focus();
				
				return false;
			});
			
			//keyboard
			///TODO: add navigation with UP and DOWN buttons
			$('.PGI_VALUE input[type="text"]', _pg).keydown(function(e){
				if(e.keyCode == 13) {//enter
					EndEdit(this);
					return false;
				}
				if(e.keyCode == 27) {//escape
					CancelEdit(this);
					return false;
				}
				return true;
			}).add('.PGI_VALUE select', _pg).blur(function(){//Cancel edit on leave
				CancelEdit(this);
			}).change(function(){//Store edited values
				EndEdit(this);
			});
			
			UpdateDescription();
			ApplyStyles();
		});
	};
})(jQuery);